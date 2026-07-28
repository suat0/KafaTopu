# Kafa Topu — Yol Haritası

**Hedef:** 2 oyunculu (insan vs insan / insan vs AI) kafa topu oyunu.
**Platform:** PC (klavye) + Mobil (dokunmatik), tek input soyutlaması ardında.
**Kalite çıtası:** Portfolyoya konacak seviye — düzgün mimari, testlenebilir çekirdek, cilalı his.

**Çalışma şekli:** Her faz için ben *ne yapılacağını ve neden* anlatırım + ipucu veririm; kodu sen yazarsın; sonra birlikte review ederiz. Kod yazmak öğrenmenin kendisi, o kısmı devretme.

---

## Bölüm 0 — Mevcut durumun review'ü

Şu an çalışan bir prototip var (top düşüyor, oyuncu sağa/sola gidiyor, temas skor veriyor, düşünce Game Over). Bunlar iyi. Aşağıdakiler ise ilerlemeden önce düzeltilmesi gereken, *hepsi öğretici* sorunlar:

### 1. `PlayerController` fiziği bypass ediyor — **en kritik olan**
```csharp
transform.position += new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0);
```
`transform.position`'ı doğrudan yazmak Rigidbody2D'yi devre dışı bırakır. Sonuçları:
- Oyuncu duvarların içinden geçer (LeftWall/RightWall onu durduramaz).
- Topa "çarpmaz", topun içine girer — çarpışma çözümü tutarsız olur.
- Kafa topunda vuruş hissinin tamamı çarpışma fiziğinden gelir; bu haliyle imkânsız.

**Yapılacak:** Hareketi `FixedUpdate` içinde `rb.velocity` ile ver. Input okumayı `Update`'te bırak, kullanmayı `FixedUpdate`'e taşı (input frame'de okunur, fizik sabit adımda çalışır).

### 2. `BallController` UI'ı tanıyor — bağımlılık yönü ters
```csharp
[SerializeField] private GameOverUI gameOverUI;
...
gameOverUI.Show();
```
Top, oyunun bittiğine karar vermemeli ve UI'ı çağırmamalı. Topun tek işi "ben aut oldum" demek. Kararı bir `GameManager` verir, UI onu dinler.

**Yapılacak:** Faz 3'teki event kanallarıyla çöz. `Ball → event → GameManager → event → UI`.

### 3. `ScoreData` bir ScriptableObject'te runtime state tutuyor
```csharp
private int score;   // [SerializeField] yok → inspector'da görünmez
```
İki ayrı sorun:
- **Editörde kalıcılık:** SO instance'ları Play Mode bittikten sonra bellekte kalır. Play'e her bastığında skor sıfırdan başlamayabilir (domain reload ayarına göre). Bilinçli bir tercih değilse bug'dır.
- **Sorumluluk karışıklığı:** SO'lar *konfigürasyon* için idealdir (max hız, gol sayısı, zorluk). *Değişen maç durumu* için ya `OnEnable`'da sıfırla ya da state'i normal bir C# sınıfına taşı.

**Not:** SO'yu "event kanalı / paylaşılan değişken" olarak kullanmak meşru ve güçlü bir pattern (Ryan Hipple, Unite 2017). Ama o zaman bilerek yapılmalı ve reset'i garanti altına alınmalı.

### 4. Magic number'lar
`-6f` (düşme sınırı), `Vector3.zero` (reset pozisyonu) koda gömülü. Kamera `orthographic size 5.4`, zemin `y = -4.5` — bu değerler birbirine bağlı ve sahne değişince kod sessizce bozulur.

**Yapılacak:** `[SerializeField]` alanlara çıkar, ya da reset noktasını sahnedeki bir `Transform` ile referansla.

### 5. `ScoreUI.Update()` her karede string üretiyor
```csharp
void Update() { scoreText.text = scoreData.GetScore().ToString(); }
```
Saniyede 60 kez `ToString()` → 60 kez heap allocation → GC baskısı. Mobilde takılma sebebi. Skor saniyede 60 kez değişmiyor.

**Yapılacak:** Event-driven yap — skor *değiştiğinde* güncelle.

### 6. `GameOverUI` `Time.timeScale`'i yönetiyor
Bir UI sınıfı oyunun zamanını durdurmamalı. Bu `GameManager`'ın / state machine'in işi. Ayrıca `timeScale = 0` iken `Update` çalışmaya devam eder ama `FixedUpdate` durur — ileride kafa karıştırır.

### 7. Küçükler
- `.gitignore`'a `.DS_Store` ekle (şu an repo'ya sızmak üzere).
- `Assets/_Project/ScoreData.asset` kök klasörde duruyor; `Assets/_Project/Data/` altına al.
- Tag/Layer düzeni yok — Faz 0'da kur.

---

## Faz 0 — Temel ve mimari kararlar ✅ TAMAMLANDI (2026-07-26)

Kod yazmadan önce zemini düzelt. Sonra dönüp düzeltmek 3 kat pahalıya patlar.

**Klasör yapısı**
```
Assets/_Project/
  Art/            Sprites, animasyonlar
  Audio/          SFX, müzik
  Data/           ScriptableObject asset'leri
  Physics/        PhysicsMaterial2D
  Prefabs/
  Scenes/         Boot, MainMenu, Game
  Scripts/
    Core/         GameManager, state machine, event kanalları
    Gameplay/     Player, Ball, Goal, AI
    Input/        IInputSource ve implementasyonları
    UI/
    Audio/
  Settings/       Input Actions asset
```

**Layer'lar:** `Player`, `Ball`, `Ground`, `Wall`, `Goal`
Project Settings → Physics 2D → collision matrix'i buna göre ayarla (örn. Player-Player çarpışmasın; her oyuncu kendi yarı sahasında kalsın).

**Paket:** Input System'i ekle (Window → Package Manager → Input System). "Both" moduna al ki eski `Input.GetAxisRaw` kodun kırılmasın, kademeli geçersin.

**Assembly Definition** (opsiyonel, portfolyo artısı): `KafaTopu.Runtime` ve `KafaTopu.Tests` asmdef'leri. Derleme süresini düşürür ve test kurulumunu netleştirir.

**Çıktı:** Yapı hazır, hiçbir davranış değişmedi, oyun hâlâ çalışıyor.

---

## Faz 1 — Karakter kontrolü (fizik doğru)

Kafa topunda oyunun %70'i burada. Kontrol iyi hissettirmiyorsa gerisi kurtarmaz.

**Yapılacaklar**
1. `PlayerController`'ı Rigidbody2D'ye taşı — `FixedUpdate` + `rb.velocity`.
   - `rb.velocity = new Vector2(input * speed, rb.velocity.y)` — y'yi ezme, yerçekimi çalışsın.
   - Rigidbody2D: `Freeze Rotation Z` açık, `Interpolate = Interpolate`, `Collision Detection = Continuous`.
2. **Ground check:** `Physics2D.OverlapCircle(feetPos, radius, groundLayer)`. Ayak noktası için child bir empty Transform kullan.
3. **Zıplama:** `rb.velocity = new Vector2(rb.velocity.x, jumpForce)` veya `AddForce(..., ForceMode2D.Impulse)`. İkisinin farkını anla — velocity ataması deterministik, force kütleye bağlı.
4. **His katkıları** (bunlar oyunu "iyi" yapan detaylar):
   - *Coyote time:* zeminden ayrıldıktan sonra ~0.1sn zıplayabilme.
   - *Jump buffer:* yere değmeden ~0.1sn önce basılan zıplama kaydedilsin.
   - *Variable jump height:* tuş bırakılınca yukarı hızı kes.
   - Yukarı çıkarken vs düşerken farklı `gravityScale` — düşüş daha hızlı, daha tok hissettirir.
5. **Kafa collider'ı:** Gövdeden ayrı, child bir `CircleCollider2D`. Üzerine daha yüksek `bounciness`'lı ayrı bir PhysicsMaterial2D koy — kafa vuruşu gövde vuruşundan farklı hissetsin.
6. **Ayak/tekme:** Child bir Transform'u tuşla döndür (basit ve okunur) veya `HingeJoint2D` + `JointMotor2D` (daha fiziksel, daha zor). Başlangıç için ilkini öner.
7. **Hareket sınırı:** Her oyuncu kendi yarı sahasında. `Mathf.Clamp` ile pozisyon kısıtlamak yerine, görünmez duvar collider'ları koy — fizikle çöz, kodla değil.

**Nasıl test edersin:** İki dakika sadece koş-zıpla. Sıkıcı geliyorsa parametreleri kurcala. Bu fazda "yeterince iyi" diye geçme.

---

## Faz 2 — Top ve vuruş hissi

**Yapılacaklar**
1. Top parametrelerini ayarla: `mass` (şu an 0.45 — gerçekçi ama oyunda ağır gelebilir), `linearDrag` (hava sürtünmesi), `bounciness`. Bunları bir `BallSettings` ScriptableObject'e taşı ki Play Mode'da kurcalayıp kaydedebilesin.
2. **Hız sınırı:** `FixedUpdate`'te `rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed)`. Yoksa top ekrandan uçar ve collider'ları atlar.
3. **Vuruş kuvveti:** Saf çarpışma fiziği yumuşak hisseder. `OnCollisionEnter2D`'de temas normaline göre ekstra impuls ekle:
   - Temas noktası (`collision.GetContact(0)`) kafa collider'ındaysa daha güçlü.
   - Oyuncunun o anki hızını topa kısmen aktar (koşarken vurmak daha sert olsun).
4. **Spin:** Yatay vuruşta `rb.angularVelocity` ver — görsel olarak çok fark eder, maliyeti sıfır.
5. **Aut yok:** Duvarlar topu içeride tutar; y < eşik kontrolü artık gereksiz (yalnızca güvenlik ağı olarak kalsın).

**Uyarı:** Bu fazda parametre kurcalamak saatler alabilir ve bu *normaldir*. Değerleri SO'ya taşımanın sebebi tam olarak bu.

---

## Faz 3 — Maç kuralları ve state machine ✅ TAMAMLANDI (2026-07-28)

**Eksik bırakılanlar (bilinçli):** Uzatma/altın gol yok — süre bitince eşitse berabere. Duraklatma *menüsü* (devam/yeniden başla/menü butonları) Faz 6'ya bırakıldı; şimdilik ESC ile açılan basit bir overlay var.

Burada oyun "prototip"ten "oyun"a döner.

**Yapılacaklar**
1. **Kaleler:** Her kalede `isTrigger` collider. `GoalTrigger` bileşeni `OnTriggerEnter2D`'de hangi tarafın gol yediğini bildirir. Kale direkleri ayrı katı collider'lar.
2. **State machine:**
   ```
   PreMatch → KickOff → Playing → GoalScored → (KickOff | MatchEnd)
                          ↕ Paused
   ```
   Basit bir `enum` + `switch` ile başla. Sınıf başına bir state'e (State pattern) ancak gerçekten şişerse geç — erken soyutlama en yaygın hatadır.
3. **Reset akışı:** Gol sonrası top ve iki oyuncu başlangıç pozisyonuna, 2-3 saniyelik geri sayım, sonra `Playing`. `Time.timeScale` ile değil, state ile yönet (input'u state kapatır).
4. **Maç süresi:** Örn. 90 saniye. `MatchTimer`'ı **saf C# sınıfı** yaz (MonoBehaviour değil) — Faz 8'de birim testi yazabilmen için.
5. **Event kanalları:** `GoalScoredEvent`, `MatchEndedEvent` gibi ScriptableObject event'ler. UI ve ses bunlara abone olur; `GameManager` UI'ı hiç tanımaz.
   - Alternatif: statik `public static event Action<int> OnGoalScored`. Daha basit ama inspector'da görünmez ve unsubscribe unutulursa sızıntı yapar. İkisini de dene, farkı gör.
6. **Beraberlik:** Süre bitince eşitse — uzatma, altın gol, ya da beraberlik. Bir tanesini seç, kural netleşsin.

**Çıktı:** Baştan sona oynanabilir bir maç. Bu, projenin ilk gerçek "bitmiş" hâli.

---

## Faz 4 — Rakip: 2. oyuncu ve AI

**Kritik tasarım kararı — bunu doğru yaparsan gerisi bedava gelir:**

`PlayerController` input'u *nereden geldiğini bilmemeli*. Bir arayüz tanımla:

```csharp
public interface IInputSource {
    float Horizontal { get; }
    bool JumpPressed { get; }
    bool KickPressed { get; }
}
```

Üç implementasyon: `KeyboardInput`, `TouchInput`, `AIInput`. `PlayerController` sadece `IInputSource` görür. Böylece AI, insan oyuncu ve mobil kontrol *aynı kod yolunu* kullanır — AI ayrı bir kod dalı olmaz, test edilmesi kolaylaşır ve Faz 5 neredeyse bedavaya gelir.

**AI davranışı (basitten karmaşığa):**
1. Topun x'ini takip et, o yöne yürü.
2. Top yeterince yakın ve yukarıdaysa zıpla.
3. Topun iniş noktasını tahmin et (basit balistik: `y = y0 + v0t - ½gt²`, t'yi çöz) ve oraya git.
4. **Zorluk seviyeleri:** Reaksiyon gecikmesi (0.3sn → 0.05sn), tahmin hata payı, hareket hızı çarpanı. Zorluk = "AI daha hızlı" değil, "AI daha az hata yapıyor" olsun.

**İpucu:** AI karar mantığını saf fonksiyona çıkar — `DecideAction(ballPos, ballVel, myPos) → InputState`. MonoBehaviour'dan bağımsız olduğu an test edilebilir hale gelir.

---

## Faz 5 — Input System (PC + mobil)

Faz 4'teki arayüz sayesinde bu faz kısa.

1. Input Actions asset oluştur: `Move` (1D axis), `Jump` (button), `Kick` (button), `Pause`.
2. Action map'ler: `Player1`, `Player2` — aynı klavyede farklı binding'ler (WASD vs Oklar).
3. **Mobil:** Ekranda buton (`On-Screen Button` bileşeni Input System'le hazır gelir) veya sol yarı = hareket / sağ yarı = zıpla. İkincisi daha temiz ama 2 oyuncu tek cihazda oynayamaz — mobilde 1P vs AI moduna kilitlemeyi düşün.
4. **Safe area:** Çentikli telefonlarda UI kesilir. Canvas'a bir `SafeAreaFitter` yaz (kısa bir script) veya hazır olanı kullan.
5. **Test:** Editörde Device Simulator (Window → General → Device Simulator) ile farklı en-boy oranlarını dene. 4:3 ve 20:9 arasında sahne bozulmasın — `Camera.orthographicSize` sabit olduğu için geniş ekranda yanlardan fazla alan görünür; duvarları ona göre konumla veya kamerayı en-boy oranına göre ayarla.

---

## Faz 6 — Sahne akışı ve UI

**Sahneler:** `Boot` (init, ayar yükleme) → `MainMenu` → `Game`.
Boot sahnesi şart değil ama kalıcı sistemler (ses, ayarlar) için temiz bir giriş noktası verir.

**Ekranlar:**
- Ana menü: 1 Oyuncu / 2 Oyuncu / Ayarlar / Çıkış
- Zorluk seçimi (1P modunda)
- HUD: skor, süre, gol anonsu
- Pause menü (devam / yeniden başla / menü)
- Maç sonu: skor, kazanan, tekrar oyna / menü

**Dikkat:**
- UI'ı `Canvas Scaler → Scale With Screen Size`, referans çözünürlük 1920x1080, `Match = 0.5` ile kur.
- Sahne geçişlerinde `Time.timeScale`'i sıfırlamayı unutma — en yaygın "menüden dönünce oyun donuk" bug'ı budur.
- Buton `onClick`'lerini inspector'dan bağlamak hızlı ama refactor'da sessizce kopar. Kritik olanları koddan bağlamayı düşün.

---

## Faz 7 — Cila (juice)

Bu faz "çalışan oyun" ile "iyi oyun" arasındaki farkın tamamı. Atlamak cazip gelir, atlama.

- **Ses:** Vuruş (kafa/ayak farklı), zıplama, iniş, direk, gol, tribün, düdük, menü tıklaması. `AudioManager` + pitch randomization (`pitch = Random.Range(0.95f, 1.05f)`) — aynı sesin tekrarı böyle gizlenir.
- **Partikül:** Vuruşta toz, golde konfeti, iniste toz bulutu.
- **Kamera:** Sert vuruşta ve golde hafif shake. Abartma — 0.15sn, küçük genlik.
- **Hit stop:** Sert vuruşta 2-3 frame'lik donma. Ucuz, çok etkili.
- **Squash & stretch:** Top ve karakter zıplarken/inerken hafif deformasyon. `transform.localScale` üzerinde basit bir coroutine yeter.
- **Gol anonsu:** Büyük yazı, slow-motion (`Time.timeScale = 0.3f` kısa süre), sonra kickoff.
- **Kalıcılık:** En yüksek skor / kazanma sayısı → `PlayerPrefs` (basit) veya JSON (`Application.persistentDataPath`, daha profesyonel).

---

## Faz 8 — Test, build, teslim

**Testler** (`Assets/Tests/EditMode/`):
- `MatchTimer` — süre doğru azalıyor mu, bitişte event tetikleniyor mu.
- `ScoreService` — gol skoru doğru artırıyor mu, reset çalışıyor mu.
- AI karar fonksiyonu — verilen top durumunda beklenen yöne gidiyor mu.

Bunları test edebilmek için o sınıfların MonoBehaviour olmaması gerekir. Faz 3 ve 4'te "saf C# sınıfı yaz" demenin sebebi buydu.

Bir PlayMode testi de ekle: top kaleye girince skor artıyor mu (entegrasyon).

**Build:**
- PC (Windows/Mac) — hızlı ve kolay.
- Android — keystore, package name, ikon, splash, orientation (landscape kilitle), min API level. İlk Android build'i her zaman beklenenden uzun sürer, buna hazır ol.

**Teslim:**
- `README.md`: ne olduğu, nasıl oynanacağı, ekran görüntüsü/GIF, kullanılan teknikler.
- Portfolyo için GIF şart — 10 saniyelik iyi bir gol klibi, 10 sayfa açıklamadan değerli.
- itch.io'ya WebGL build atmayı düşün (tarayıcıda oynanabilir olması portfolyoda çok işe yarar).

---

## Öncelik sırası

Vakit daralırsa şu sırayla feda et:

| Öncelik | Faz |
|---|---|
| **Vazgeçilmez** | 0, 1, 2, 3 — bunlar olmadan oyun yok |
| **Çok önemli** | 4 (AI), 6 (UI akışı) — bunlarsız "ürün" değil, demo |
| **Önemli** | 7 (cila) — kaliteyi belirleyen faz |
| **Sonraya kalabilir** | 5 (mobil), 8 (test/Android build) |

---

## Sıradaki adım

**Faz 4 — `IInputSource` soyutlaması ve AI rakip.**

Faz 0–3 tamam: proje yapısı, oyuncu fiziği, top vuruş hissi, maç kuralları.
Sahada tek oyuncu var; ikinci oyuncu ve AI Faz 4'te geliyor.

İlk iş `IInputSource` arayüzü — `PlayerController` input'un nereden geldiğini
bilmemeli. Bu karar Faz 5'i (mobil) neredeyse bedavaya getirir.
