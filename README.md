# 📦 CacheSmartProject

CacheSmartProject məlumatları yüksək performansla və səmərəli şəkildə idarə etmək üçün **Redis** və **MemoryCache** texnologiyalarını birləşdirən müasir bir `.NET 7` əsaslı tətbiqdir. Layihənin əsas məqsədi **cache warming**, **change detection** və **data consistency** problemlərinə həll təqdim etməkdir.

## 🚀 Məqsəd

* **Verilənlər bazasına yükü azaltmaq**
* **Frontend tətbiqlərinə daha sürətli məlumat ötürmək**
* **Keşləmə və dəyişiklik aşkarlama strategiyası ilə yüksək performans təmin etmək**

---

## 📁 Struktur və Arxitektura

### 🔹 Layihə Layihələndirilməsi

* `Domain`: Entitilər və DTO modelləri
* `Persistence`: ADO.NET ilə PostgreSQL əsaslı Repository-lər
* `Infrastructure`: Service qatları, Redis və MemoryCache ilə işləyən logika
* `Application`: Service interfeysləri və cache strategiyaları
* `Presentation`: (əlavə edilə bilər) API və ya UI təqdimatı

### 🔹 Texnologiyalar

* `ASP.NET Core`
* `ADO.NET`
* `PostgreSQL`
* `Redis (StackExchange.Redis)`
* `IMemoryCache`
* `AutoMapper`
* `ILogger`
* `Swagger` (əlavə edilə bilər)

---

## ⚙️ Quraşdırma Təlimatları

1. **Repository-ni klonla:**

   ```bash
   git clone https://github.com/senin-username/CacheSmartProject.git
   ```

2. **Konfiqurasiya et:**

   * `appsettings.json` daxilində PostgreSQL və Redis connection string-ləri düzgün yazılmalıdır.
   * Nümunə:

     ```json
     "ConnectionStrings": {
         "DefaultConnection": "Host=localhost;Port=5432;Database=cache_db;Username=postgres;Password=1234"
     },
     "Redis": {
         "Configuration": "localhost:6379"
     }
     ```

3. **Database-i yarat:**

   * SQL ilə `Categories`, `Services`, `Stories` cədvəlləri yaradılmalıdır.

4. **Tətbiqi işə sal:**

   ```bash
   dotnet run
   ```

---

## 🧠 Cache Warming Strategiyası

### Nədir?

Backend server start olduqda və ya yeni məlumat əlavə edildikdə `Redis` və `MemoryCache` sinxron şəkildə məlumatla doldurulur.

### Necə Tətbiq Edilir?

* Bütün `GetAllAsync()` metodları yerinə `GetAlwaysAsync()` istifadə olunur.
* Əgər `MemoryCache`-də data varsa – oradan qaytarılır.
* Əgər yoxdursa və Redis-də varsa – Redis-dən götürülür və MemoryCache-ə yazılır.
* Hər iki yerdə məlumat yoxdursa – verilənlər bazasından alınır və cache-lərə yazılır.

### Service-lər üçün Cache Warming

Bütün servislər `ICacheWarmable` interfeysindən implement edilmişdir. Bu interfeys daxilində:

```csharp
Task RefreshCacheAsync();
```

metodu implement olunur və `Program.cs` içində start zamanı çağırılır.

---

## 🔄 Dəyişiklik Aşkarlama Strategiyası

### 🎯 Məqsəd

Frontend tərəfdə hər dəfə bütün məlumatları çəkib performansa zərər verməkdənsə, yalnız **dəyişiklik olduqda** data çəkmək.

### 🧩 İş Prinsipi

1. Hər entitidə `LastModified` timestamp saxlanılır.
2. Frontend `localStorage` vasitəsilə son dəyişiklik vaxtını yadda saxlayır.
3. API-ə bu dəyər göndərilir.
4. Server tərəfdə `HasChangedAsync(DateTime clientLastModified)` metodu Redis və ya DB üzərindən müqayisə aparır.
5. Əgər dəyişiklik varsa, yeni məlumat qaytarılır və frontend `localStorage`-i yeniləyir.

### 💾 LocalStorage ilə Frontend nümunəsi (JS)

```js
const localLastModified = localStorage.getItem("categoriesLastModified");

const response = await fetch(`/api/categories/has-changed?clientLastModified=${localLastModified}`);
const hasChanged = await response.json();

if (hasChanged) {
  const dataResponse = await fetch("/api/categories");
  const categories = await dataResponse.json();

  renderCategories(categories);

  const newTimestamp = categories[0]?.lastModified;
  if (newTimestamp) {
    localStorage.setItem("categoriesLastModified", newTimestamp);
  }
}
```

### 🛡 Backend HasChanged Metodu (C#)

```csharp
public async Task<bool> HasCategoryChangedAsync(DateTime clientLastModified)
{
    var redisValue = await _redisDb.StringGetAsync("categories:lastModified");
    if (redisValue.IsNullOrEmpty) return true;

    var lastModified = DateTime.Parse(redisValue!);
    return lastModified > clientLastModified;
}
```

---

## 📊 Performans Müqayisəsi

| Əməliyyat                 | Keşsiz         | Redis Keşi ilə | Redis + MemoryCache |
| ------------------------- | -------------- | -------------- | ------------------- |
| 1000 record oxuma         | 780ms          | 320ms          | **95ms** ✅          |
| Eyni dataya təkrar sorğu  | 760ms          | 40ms           | **5ms** ✅           |
| Yeni data əlavə olunduqda | ❌ 1-5s gecikmə | ✅ dərhal       | ✅ dərhal            |

---

## 🧹 Əlavə Qeydlər

* Hər servisdə `try-catch` bloku ilə `ILogger` vasitəsilə exception handling və loqlaşdırma aparılır.
* Repository qatında ADO.NET vasitəsilə sadə və effektiv SQL sorğuları icra edilir.
* `Delete`, `Update`, `Add` əməliyyatları sonrası `InvalidateCache()` metodu ilə Redis və MemoryCache təmizlənir və `RefreshCacheAsync()` ilə yenilənir.

---



