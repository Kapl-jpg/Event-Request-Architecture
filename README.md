# Kapl Event Driven Architecture Documentation
## 1. Introduction
### 1.1 Overview
This framework implements an event-driven architecture for Unity, enabling you to trigger events from anywhere in your project. 
The system decouples components, making your project more modular, scalable, and easier to maintain. 
To use events and data requests, the containing classes must inherit from the base class Subscriber.
### 1.2 Key Features
Global Event Invocation: Call events from any part of your code.
Flexible Subscription Options: Three ways to register event handler methods.
Optional Data Passing: Events can be invoked with or without data.
Data Request Mechanism: Retrieve and update shared data from anywhere in your project.
## 2. Installation
The installation is done through the Unity module. Go to `Window > Package Manager > + > Install package from git URL...` and paste the link:
```
git clone https://github.com/Kapl-jpg/EventDrivenArchitecture.git
```
## 3. Working with Events
### 3.1 Registering Methods as Event Handlers
>[!WARNING]
>To make a method an event handler, you must:
>* Decorate the method with the `[Event(...)]` attribute.
>* Ensure the class containing the method inherits from `Subscriber`.
### 3.2 Three Usage Variants for the [Event] Attribute
#### 3.2.1 Basic Event Registration by Name
```
[Event("EventName")]
private void OnEventName(object data)
{
    // This method will be invoked for every instance where "EventName" is registered.
}
```
#### 3.2.2 Group Registration
```
[Event("GroupName", "EventName")]
private void OnGroupedEvent(object data)
{
    // This method will be invoked for events within the "GroupName" group and having the "EventName" identifier.
}
```
This approach allows you to reuse the event name within a specific group. The event can be triggered for the entire group or for a particular event in that group.

#### 3.2.3Local Registration for a Specific Object
```
[Event(true, "EventName")]
private void OnLocalEvent(object data)
{
    // This method will be triggered only for this specific object.
}
```
>[!IMPORTANT]
>*Note*: It’s not mandatory to pass data when invoking events. You can simply call the event without any parameters.
### 3.3 Publishing (Invoking) Events
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

### 3.4 Example: Applying Damage
Below is an example of an event for applying damage. Notice that you can choose whether to pass data or simply invoke the event.
```
// The class must inherit from Subscriber
public class DamageReceiver : Subscriber
{
    // Method registered for the "ApplyDamage" event
    [Event("ApplyDamage")]
    private void ApplyDamage(object data)
    {
        if (data is int damage)
        {
            Debug.Log($"Damage received: {damage}");
            // Implement damage logic here (e.g., reduce health)
        }
        else
        {
            // Event invoked without data
            Debug.Log("ApplyDamage event called without data.");
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
## 4. Working with Data Requests
### 4.1 Data Request Mechanism
>[!WARNING]
>The framework allows you to retrieve data from different parts of your system via requests. For this:
>* Mark the field with the `[Request("RequestName")]` attribute.
>*Ensure the field is of type `ObservableField<TypeField>` and properly initialized (e.g., `new ObservableField<TypeField>()`).
>*The class containing the field must inherit from `Subscriber`.
### 4.2 Accessing and Updating Data
Retrieve a value using:
```
var value = RequestManager.GetValue<TypeField>("RequestName");
```
To update the value, access the field directly through its `.value` property.
### 4.3 Example: Managing Points
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
## 5. Additional Recommendations
* **Inheritance from `Subscriber`**:
All classes containing methods decorated with `[Event]` or fields with `[Request]` must inherit from `Subscriber` to ensure proper registration and handling.
* **Access Modifiers**:
Event handler methods can be private or public. Similarly, fields used for data requests can have any access level (private, public, or marked with `[SerializeField]` for editor visibility).
* **Data Passing Flexibility**:
When invoking events, you may pass any type of data. However, it’s completely acceptable to trigger an event without passing data.
## 6. Comprehensive Example
The following example combines both event handling and data requests:
```
using UnityEngine;

public class DamageSystem : Subscriber
{
    // Event handler for applying damage
    [Event("ApplyDamage")]
    private void OnApplyDamage(object data)
    {
        if (data is int damage)
        {
            Debug.Log($"Damage applied: {damage}");
            // Update character health, etc.
        }
        else
        {
            Debug.Log("ApplyDamage event called without data.");
        }
    }
}

public class PlayerStats : Subscriber
{
    // Field for storing points, accessible via the "Points" request
    [Request("Points")]
    [SerializeField]
    private ObservableField<int> points = new ObservableField<int>();

    // Method to increase player points
    public void IncreasePoints(int value)
    {
        points.value += value;
        Debug.Log($"Player points: {points.value}");
    }
}

public class GameController : MonoBehaviour
{
    private void Start()
    {
        // Example: Publishing the ApplyDamage event with data
        int damageAmount = 15;
        EventManager.Publish("ApplyDamage", damageAmount);

        // Example: Publishing the ApplyDamage event without data
        EventManager.Publish("ApplyDamage");

        // Example: Accessing points via data request
        int currentPoints = RequestManager.GetValue<int>("Points");
        Debug.Log($"Points retrieved via request: {currentPoints}");
    }
}
```
## 7. Conclusion
The Unity Event Driven Framework provides a flexible system for component interaction using events and data requests. 
With three distinct methods for event registration, you can tailor the scope of event invocation—be it global, grouped, or object-specific. 
Additionally, events can be triggered with or without data, according to your needs. 
This architecture improves modularity, scalability, and simplifies debugging in Unity projects.
