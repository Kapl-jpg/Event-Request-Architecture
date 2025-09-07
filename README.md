# Kapl Event-Request Architecture Documentation
## 1. Introduction
### 1.1 Overview
This framework implements an event - request architecture for Unity, enabling you to trigger events from anywhere in your project. 
The system decouples components, making your project more modular, scalable, and easier to maintain.
### 1.2 Key Features
* Global Event Invocation: Call events from any part of your code.
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
1. Go to `Tools > Event-Request > Generate Names Scripts`
2. Click the button to generate the scripts.
3. Navigate to the created folder `Assets > Scripts > Names` and open the generated scripts.
4. Define your event and request names as constants inside the scripts.
5. Use these constants instead of raw strings when publishing or subscribing to events or requests.
## 4. Events System
### 4.1 Registering Event Handlers
>[!WARNING]
Always try to register event subscriptions inside Awake or Start. Avoid multiple event registration, as this may lead to false method calls.
### 4.2 Subscribing to Events
#### 4.2.1 Basic Event Subscription
```
private void Awake()
{
    EventManager.Subscribe("EventName", gameObject, OnEventName);
}

private void OnEventName()
{
    // Called whenever "EventName" is invoked.
}

```
#### 4.2.2 Typed Event Subscription
```
private void Awake()
{
    EventManager.Subscribe<string>("EventName", gameObject, OnEventName);
}

private void OnEventName(string message)
{
    // Called when "EventName" is invoked with a string argument.
}

```
This approach allows you to reuse the event name within a specific group. The event can be triggered for the entire group or for a particular event in that group.

#### 4.2.3 Object-Specific Subscription
```
private void Awake()
{
    var id = gameObject.GetInstanceID();
    EventManager.Subscribe($"EventName.{id}", gameObject, OnEventName);
}

private void OnEventName()
{
    // Called only for this specific object.
}
```
### 4.3 Triggering Events
To trigger an event, use the EventManager.Publish method. Depending on the type of subscription, use the following approaches:

* **Standard Event**:
```
// Trigger without data
EventManager.Trigger("EventName");

// Trigger with data
EventManager.Trigger("EventName", someData);
```
* **Object-Specific Event**:
```
// Trigger without data
EventManager.Publish($"EventName.{targetGameObject.GetInstanceID()}");

// Trigger with data
EventManager.Publish($"EventName.{targetGameObject.GetInstanceID()}", someData);
```
### 4.4 Example: Applying Damage
Below is an example of an event for applying damage. Notice that you can choose whether to pass data or simply invoke the event.
```
public class DamageReceiver : Monobehaviour
{
    private void Awake()
    {
        EventManager.Subscribe("ApplyDamage", gameObject, ApplyDamage);
        EventManager.Subscribe<int>("ApplyDamage", gameObject, ApplyDamage);
    }
    
    private void ApplyDamage()
    {
        Debug.Log($"Damage received: {1}");
    }
    
    private void ApplyDamage(int damage)
    {
        Debug.Log($"Damage received: {damage}");
    }
}
```
To trigger the event from anywhere in your code, you can do either:
```
// Without data
EventManager.Publish("ApplyDamage");

// With data
EventManager.Publish("ApplyDamage", 10);
```
>[!IMPORTANT]
>*Note*: 
>* Passing data is optional.
>* You can use any data type (int, float, string, custom classes, etc.).
>* Subscribing with GetInstanceID ensures the event is delivered only to a specific object.
## 5. Request System
### 5.1 Accessing and Updating Data
To register a request, use RequestManager.SetValue.
You can assign any type of data:
```
RequestManager.SetValue("PlayerHealth", 100);
```
To update the value, access the field directly through its `.value` property.
### 5.2 Retrieving Request Values
```
int health = RequestManager.GetValue<int>("PlayerHealth");
```
### 5.3 Example: Player Stats
Below is a simple example of storing and retrieving player stats:
```
public class PlayerStats : MonoBehaviour
{
    private void Awake()
    {
        RequestManager.SetValue("PlayerHealth", 100);
        RequestManager.SetValue("PlayerName", "Knight");
    }

    private void Start()
    {
        int health = RequestManager.GetValue<int>("PlayerHealth");
        string name = RequestManager.GetValue<string>("PlayerName");

        Debug.Log($"Player {name} has {health} HP.");
    }
}
```
>[!IMPORTANT]
>* Requests are global and can be accessed from anywhere in the project.
>* You can store and retrieve any data type (int, float, string, custom classes, etc.).
>* Overwriting a request with the same key will replace the old value.
## 6. Conclusion
The Kapl Event - Request Framework provides a flexible system for component interaction using events and data requests. 
With three distinct methods for event registration, you can tailor the scope of event invocation—be it global, grouped, or object-specific. 
Additionally, events can be triggered with or without data, according to your needs. 
This architecture improves modularity, scalability, and simplifies debugging in Unity projects.
## 7. Recent Changes
* The event-request system has been redesigned.
* Event registration has been simplified.
* Added a debugging tool `Tools -> Event-Request -> Show Dependencies`.
