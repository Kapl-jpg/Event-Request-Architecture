## Kapl Event-Request Framework Documentation


### 1. Introduction

A lightweight **Event** and **Request** system for Unity. It uses C# `Action` events under the hood and requires no base classes or custom attributes. Features:

* **Global and object-scoped events**
* **Strict type-safe subscriptions**
* **Automatic unsubscription** when GameObjects are destroyed
* **Data sharing** with `ObservableField<T>`
* **Persistent temporary requests** across scene loads

> [!NOTE]
> **Namespace:** all API lives under the `ERA` namespace.

> [!WARNING]
> **Avoid** firing events or requests inside `Update`, `FixedUpdate`, or `LateUpdate` loops to prevent performance issues.

---

### 2. Installation

Install via Unity Package Manager:

1. **Window > Package Manager > + > Add package from Git URL...**
2. Paste:

   ```
   https://github.com/Kapl-jpg/Event-Request-Architecture.git#StableVersion
   ```
3. Click **Add**.

---

### 3. Event System

#### 3.1 Registering and Triggering

```csharp
// Subscribe a method to an event with no args
EventManager.AddEvent("GameOver", gameObject, OnGameOver);

// Subscribe a method to an event with one arg (e.g. damage)
void OnDamage(int dmg) { /* ... */ }
EventManager.AddEvent<int>("TakeDamage", gameObject, OnDamage);

// Trigger globally
EventManager.Trigger("GameOver");
EventManager.Trigger("TakeDamage", 10);

// Trigger for specific object
int id = target.GetInstanceID();
EventManager.Trigger($"{id}.TakeDamage", 5);
```

> [!TIP]
> * The `ownerGameObject` is almost always `this.gameObject`. It binds the handler’s lifetime to that object.
> * For one‑arg events, `T` can be any type, including tuples like `(int, string, GameObject)`.

#### 3.2 Removing Handlers Manually

```csharp
// Remove a specific handler if needed
EventManager.RemoveEvent("SomeEvent", OnSomeEvent);
```

> [!TIP]
> Manual removal helps when you must unsubscribe before destruction (e.g. dynamic systems), though automatic cleanup usually suffices.

---

### 4. Request System

`ObservableField<T>` exposes a value with change notifications and syncs with `RequestManager`.

#### 4.1 Initialization

```csharp
// If you serialize in inspector:
[SerializeField] private ObservableField<int> counter;

// Or create in code:
private ObservableField<float> speed = new ObservableField<float>();
```

> [!IMPORTANT]
> If not serialized, you must instantiate with `= new ObservableField<T>()` before using. Always access via `.Value`.

#### 4.2 Persistent vs. Temp Requests

```csharp
// Scene‑local request (cleared on scene load)
counter.Init("player.counter", gameObject);

// Cross‑scene persistent request
counter.InitTemp("global.counter");
```

On `Value` set, RequestManager is automatically updated for all subscribers.

#### 4.3 RequestManager API

```csharp
// Safe access:
if (RequestManager.TryGetValue("player.counter", out int points)) 
{
    Debug.Log(points);
}
```

---

### 5. Usage Examples

#### 5.1 Events: Publisher and Subscriber

```csharp
// Publisher
public class DamageDealer : MonoBehaviour 
{    
    public GameObject target;
    
    void Attack() 
    {
        int dmg = 20;
        int id = target.GetInstanceID();
        EventManager.Trigger($"{id}.TakeDamage", dmg);
    }
}

// Subscriber
public class DamageReceiver : MonoBehaviour 
{    
    private void Awake() 
    {
        EventManager.AddEvent<int>($"{gameObject.GetInstanceID()}.TakeDamage", gameObject, HandleDamage);
    }
    
    void HandleDamage(int damage)
    {
        Debug.Log($"Took {damage}");
    }
}
```

#### 5.2 Requests: Producer and Consumer

```csharp
// Publisher
public class ScoreProducer : MonoBehaviour 
{
    [SerializeField] private ObservableField<int> tempScore; // = 100
    [SerializeField] private ObservableField<float> score; // = 50.0f
    
    private void Awake() 
    {
        tempScore.InitTemp("session.tempScore");
        score.Init("session.score");
    }
}

// Subscriber
public class ScoreConsumer : MonoBehaviour 
{
    private void Start() 
    {
        GetScore();
    }
    private void GetScore()
    {
        RequestManager.TryGetValue("session.tempScore", out int tempScore);
        RequestManager.TryGetValue("session.score", out float score);
        
        Debug.Log("Temp score = " + tempScore); // Print: Temp score = 100
        Debug.Log("Score = " + score); // Print: Score = 50.0
    }
}
```

---

### 6. Notes & Best Practices

* All API lives in the `ERA` namespace.
* Events and requests use `Action` under the hood—avoid in tight update loops.
* Use `GetInstanceID()` for object‑specific routing.
* Instantiate non‑serialized `ObservableField<T>` with `new()`.

---

### 7. Recent Changes

* **Reworked** subscription/unsubscription: no base classes needed.
* **Removed** all custom attributes (`[Event]`, `[Request]`, `[TempRequest]`).
* **Removed** ScriptableObject registry feature.
