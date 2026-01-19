# Unity Game Demo

[![](https://markdown-videos-api.jorgenkh.no/youtube/6MAQlIkuKoM)](https://youtu.be/6MAQlIkuKoM)
<br>
[Demo Youtube Videosu](https://youtu.be/6MAQlIkuKoM)
<br><br>
[Windows Build](https://github.com/knightgames-tr/PlayableAdsDev/blob/main/windows_build.zip)

### Unity 3D ve C# dili kullanılarak bir demo hazırlanmıştır.

##### Hazırlayan: Ecem Özdoğan

**Kullanılan Ek Assetler:** Dotween, Joystick pack 

#### Geliştirme Notları:

* Manager sınıfları Singleton olarak kullanıldı.
* Ayakta durma noktaları için "StandPoint" isimli bir parent sınıf, ve bu sınıftan extend eden "ActionPoint" ve "PaymentPoint" sınıfları tanımlandı.
* NPC'lerin sıralanması için bir "LineController" sınıfı tanımlandı. NPC'ler dinamik bir offset değerine göre sıralandı.
* Kuyruktaki NPC, işlemini tamamladığı zaman "LevelManager"'a haber verir, ve sıradaki NPC harekete geçer.
* Merdiven hareketleri, her step objesinin, başa saran bir döngü içerisinde bir sonraki noktaya hareketlenmesi ile oluşturuldu.
* Tahta boyama özelliği gerçekleştirilirken, öncelikle dokunulan noktaya denk düşen piksel bulunur. Bu pikselin komşu pikselleri, brush size yarıçap olarak kullanılarak hesaplanır ve texture üzerindeki pikseller boyanır. O pikselin önceden beyaz olup olmadığı kontrol edilir ve bu kontrole göre boyama yüzdesi hesaplanır.
* Oyuncu kontrolleri character controller kullanılarak yapıldı.
* Trail ve para harcama efektleri, particles system kullanılarak yapıldı.
* Kamera geçişleri Cinemachine ile yapıldı.
* Çeşitli animasyonlar için, zincirlenmiş dotween tween özellikleri kullanıldı.
* Tutma ve yürüme animasyonlarını aynı anda çalıştırabilmek için, avatar mask kullanıldı. Animation Layer oluşturuldu.
* Bölüme özgü, daha spesifik, hard coded gerektiren aksiyonlar LevelManager üzerinde tanımlandı. Coroutine'ler kullanıldı.
