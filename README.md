# Kapl Event-Request Architecture Documentation
## 1. Introduction
### 1.1 Overview
This framework implements an event-driven architecture for Unity, enabling you to trigger events from anywhere in your project. 
The system decouples components, making your project more modular, scalable, and easier to maintain. 
To use events and data requests, the containing classes must inherit from the base class Subscriber.
### 1.2 Key Features
* Global Event Invocation: Call events from any part of your code.
* Flexible Subscription Options: Three ways to register event handler methods.
* Optional Data Passing: Events can be invoked with or without data.
* Data Request Mechanism: Retrieve and update shared data from anywhere in your project.
## 2. Installation
The installation is done through the Unity module. Go to `Window > Package Manager > + > Install package from git URL...` and paste the link:
```
https://github.com/Kapl-jpg/Event-Request-Architecture.git
```
## 3. Using the Generate Names Scripts Tool
To simplify event name management, a tool has been added under Tools/Generate Names Scripts. This tool automatically generates two scripts inside a designated folder, allowing you to define and organize event names in a structured way.
### Why Use This Tool?
* **Avoid Hardcoded Strings**: Instead of manually typing event names as strings, you can reference predefined constants, reducing the risk of typos.
* **Easier Refactoring**: If an event name changes, you only need to update it in one place.
* **Better Code Organization**: Keeps event names structured and easy to find.
### How to Use It
1. Go to `Menu items > Tools > Event-Request > Generate Names Scripts`
2. Click the button to generate the scripts.
3. Navigate to the created folder `Assets > Scripts > Names` and open the generated scripts.
4. Define your event and request names as constants inside the scripts.
5. Use these constants instead of raw strings when publishing or subscribing to events or requests.
## 4. Working with Events
### 4.1 Registering Methods as Event Handlers
>[!WARNING]
>To make a method an event handler, you must:
>* Decorate the method with the `[Event(...)]` attribute.
>* Ensure the class containing the method inherits from `Subscriber`.
### 4.2 Three Usage Variants for the [Event] Attribute
#### 4.2.1 Basic Event Registration by Name
```
[Event("EventName")]
private void OnEventName(object data)
{
    // This method will be invoked for every instance where "EventName" is registered.
}
```
#### 4.2.2 Group Registration
```
[Event("GroupName", "EventName")]
private void OnGroupedEvent(object data)
{
    // This method will be invoked for events within the "GroupName" group and having the "EventName" identifier.
}
```
This approach allows you to reuse the event name within a specific group. The event can be triggered for the entire group or for a particular event in that group.

#### 4.2.3 Local Registration for a Specific Object
```
[Event(true, "EventName")]
private void OnLocalEvent(object data)
{
    // This method will be triggered only for this specific object.
}
```
>[!IMPORTANT]
>*Note*: It’s not mandatory to pass data when invoking events. You can simply call the event without any parameters.  
You can pass any type of data, not just 'object', as shown in the examples
### 4.3 Publishing (Invoking) Events
To trigger an event, use the EventManager.Publish method. Depending on the type of subscription, use the following approaches:

* **Standard Event**:
```
EventManager.Publish("EventName", someData);
```
This call triggers all handlers registered with `[Event("EventName")]`. If you do not need to pass any data, simply omit the data parameter:
```
EventManager.Publish("EventName");
```
* **Group Event**:
```
// With data
EventManager.Publish("GroupName.EventName", someData);

// Without data
EventManager.Publish("GroupName.EventName");
```
This triggers handlers subscribed for the group "GroupName" with the event "EventName".
* **Object-Specific Event**:
```
// With data
EventManager.Publish($"{targetGameObject.GetInstanceID()}.EventName", someData);

// Without data
EventManager.Publish($"{targetGameObject.GetInstanceID()}.EventName");
```
This call triggers the event only for the object with the specified instance ID.

### 4.4 Example: Applying Damage
Below is an example of an event for applying damage. Notice that you can choose whether to pass data or simply invoke the event.
```
// The class must inherit from Subscriber
public class DamageReceiver : Subscriber
{
    // Method registered for the "ApplyDamage" event
    [Event("ApplyDamage")]
    private void ApplyDamage(int damage)
    {
        if (data is int damage)
        {
            Debug.Log($"Damage received: {damage}");
            // Implement damage logic here (e.g., reduce health)
        }
    }
}
```
To trigger the event from anywhere in your code, you can do either:
```
// With data
EventManager.Publish("ApplyDamage", 10);

// Without data
EventManager.Publish("ApplyDamage");
```
## 5. Working with Data Requests
### 5.1 Data Request Mechanism
>[!WARNING]
>The framework allows you to retrieve data from different parts of your system via requests. For this:
>* Mark the field with `[Request("RequestName")]` or `[TempRequest("TempRequestName")]` attribute.
>* Ensure the field is of type `ObservableField<TypeField>` and properly initialized (e.g., `new ObservableField<TypeField>()`).
>* The class containing the field must inherit from `Subscriber`.
### 5.2 Accessing and Updating Data
Retrieve a value using:
```
var value = RequestManager.GetValue<TypeField>("RequestName");
```
To update the value, access the field directly through its `.value` property.
### 5.3 Example: Managing Points
Below is an example where a points field is accessed and updated through a data request.
```
// The class must inherit from Subscriber
public class ScoreManager : Subscriber
{
    // Field holding the points, accessible via the "Points" request
    [Request("Points")]
    [SerializeField]
    private ObservableField<int> points = new ObservableField<int>();

    // Method to add points
    public void AddPoints(int amount)
    {
        points.value += amount;
        Debug.Log($"Updated points: {points.value}");
    }
}
```
Elsewhere in your project, retrieve the current points like this:
```
int currentPoints = RequestManager.GetValue<int>("Points");
Debug.Log($"Current points: {currentPoints}");
```
>[!IMPORTANT]
>* When using the [Request] attribute, data is stored while the object is active.
>* When using the [TempRequest] attribute, the data will be stored even if the object is not active.
## 6. Woring with Scriptable Object requests
>[!WARNING]
>The framework allows you to retrieve data from different parts of your system via requests. For this:
>* Create a ScriptableObject asset (using the `[CreateAssetMenu]` attribute) that holds a list of key–ScriptableObject pairs.
>* The ScriptableObjectRegistry asset must reside in a folder named Resources (e.g., `Assets/Resources/SORegistry.asset`) so that it can be loaded at runtime.
>* Each entry in the registry should have a unique key. This key will be used by SOManager to retrieve the associated ScriptableObject.
>* Ensure that the type requested in the call to SOManager.Get<T>(key) matches the actual type of the ScriptableObject stored in the registry.
### 6.1 Editor Tools for SORegistry
1. Go to `Menu items > Tools > Event-Request > SO Registry`
2. If the SORegistry asset does not exist yet, click “Create a new registry” to generate one.
3. Once the registry is open, add the desired ScriptableObjects and assign them unique names. These names will be used as keys for later reference.
>[!TIP]
> You can also change the data by going to the `Assets/Resources/SORegistry.asset` path.
### 6.2 Accessing and Updating Data
Retrieve a value using:
```
var myData = SOManager.Get<MyScriptableObject>("SOName");
```
### 6.3 Example Usage
```
[CreateAssetMenu(menuName = "Data", fileName = "GameSettings")]
public class GameSettings : ScriptableObject {
    public int maxPlayers = 4;
}

// Example MonoBehaviour that uses SOManager to access GameSettings
public class GameController : MonoBehaviour {
    private GameSettings settings;
    
    private void Start() {
        // Attempt to retrieve the GameSettings ScriptableObject from the registry
        settings = SOManager.Get<GameSettings>("GameSettings");
        if (settings != null) {
            Debug.Log($"Max players: {settings.maxPlayers}");
        }
    }
}
```
>[!IMPORTANT]
>* The SORegistry.asset must be located in the Resources folder to ensure it is available at runtime.
>* Lazy loading minimizes performance overhead, but it also means that any issues with loading the registry (e.g., a missing asset) will only surface when you attempt to access a ScriptableObject.
>* Maintain unique keys within the registry to avoid conflicts and unexpected behavior.
## 7. Additional Recommendations
* **Inheritance from `Subscriber`**:
All classes containing methods decorated with `[Event]` or fields with `[Request]` or `[TempRequest]` must inherit from `Subscriber` to ensure proper registration and handling.
* **Access Modifiers**:
Event handler methods can be private or public. Similarly, fields used for data requests can have any access level (private, public, or marked with `[SerializeField]` for editor visibility).
* **Data Passing Flexibility**:
When invoking events, you may pass any type of data. However, it’s completely acceptable to trigger an event without passing data.
## 8. Comprehensive Example
The following example combines both event handling and data requests:
### 8.1 Receiving data and calling events
```
public class DamageSystem : Subscriber
{
    // Event handler for applying damage
    [Event("ApplyDamage")]
    private void OnApplyDamage(object data)
    {
        if (data is int damage)
        {
            Debug.Log($"Damage applied: {damage}");
            // Update character health or perform other actions here
        }
        else
        {
            Debug.Log("ApplyDamage event called without data.");
        }
    }
}

public class TempData : Subscriber
{
    // Field available via the "GlobalPoints" temporary request
    [TempRequest("GlobalPoints")]
    [SerializeField] private ObservableField<int> points = new ObservableField<int>();

    private IEnumerator Start()
    {
        // Wait for 1 second before proceeding
        yield return new WaitForSeconds(1f);
        // Print a message to the console before deleting the object
        print("The temporary object has been deleted");
        // Destroy this game object from the scene
        Destroy(gameObject);
    }
}

public class PlayerStats : Subscriber
{
    // Field for storing points, accessible via the "Points" request
    [Request("Points")]
    [SerializeField] private ObservableField<int> points = new ObservableField<int>();
    
    // Method to increase player points
    public void IncreasePoints(int value)
    {
        points.value += value;
        // Log the current points to the console
        Debug.Log($"Player points: {points.value}");
    }
}

public class GameController : MonoBehaviour
{
    private void Start()
    {
        // Example: Publish the ApplyDamage event with data
        int damageAmount = 15;
        EventManager.Publish("ApplyDamage", damageAmount);

        // Example: Publish the ApplyDamage event without data
        EventManager.Publish("ApplyDamage");

        // Example: Retrieve points value via a request
        int currentPoints = RequestManager.GetValue<int>("Points");
        Debug.Log($"Points retrieved via request: {currentPoints}");
    }
}
```
### 8.2 Getting Scriptable Object data from scripts
```
// CreateAssetMenu attribute allows creating instances of SomeData from the Unity Editor
[CreateAssetMenu(menuName = "Data", fileName = "SomeData")]
public class SomeData : ScriptableObject
{
    // Field to store points, default value is 100
    public int points = 100;
}

public class SOTest : MonoBehaviour
{
    // Reference to hold the loaded SomeData instance
    private SomeData _obj;

    private void Start()
    {
        // Attempt to retrieve the SomeData object by its key from SOManager
        _obj = SOManager.Get<SomeData>("SomeData");
        
        // If the object is successfully retrieved, print its points value to the console
        if (_obj)
        {
            print(_obj.points);
        }
    }
}
```
## 9. Conclusion
The Kapl Event Request Framework provides a flexible system for component interaction using events and data requests. 
With three distinct methods for event registration, you can tailor the scope of event invocation—be it global, grouped, or object-specific. 
Additionally, events can be triggered with or without data, according to your needs. 
This architecture improves modularity, scalability, and simplifies debugging in Unity projects.
## 10. Recent Changes
* Added the [TempRequest] attribute, allowing data to persist even when the related objects are destroyed.
* Introduced SORegistry, providing a centralized and convenient way to access ScriptableObjects from your scripts.
