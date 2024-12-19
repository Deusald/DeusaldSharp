# DeusaldSharp

Deusald Sharp is a library that contains useful functionalities when creating game in C#.

## Build

### [NuGet](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/v/DeusaldSharp?color=blue)](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/dt/DeusaldSharp)](https://www.nuget.org/packages/DeusaldSharp/)

### Unity Install

You can add this library to Unity project in 2 ways:

1. In package manager add library as git repository using format:
   `https://github.com/Deusald/DeusaldSharp.git?path=/UnityPackage.NetStandard2.1#vX.X.X`
2. Add package using Scoped Register:
    1. TODO

## Features

* Game Server Clock in two versions: Standard and Precise
* Clear C# Coroutines system similar to one that is build in Unity Engine
* Useful Enum/String/Task/List Extensions and Helpers
* Useful Math utils
* Messages system for sending messages to classes that can subscribe for specific messages
* 2D Spline class
* Vector2, Vector3 and Typed Vector2/Vector 3 classes with all classic vector math in it

## Game Server Clock

Library contains two implementations of game server clock: precision one (that is more precise but eats more resources)
and standard one. Clocks can be used to execute game server logic in fixed time ticks.

### Precision Clock

```csharp
PrecisionClock serverClock = new PrecisionClock(50);
serverClock.Tick += frameNumber =>
{
    // Game Logic
};
serverClock.TooLongFrame += () => Console.WriteLine("Too long frame");
serverClock.Kill();
```

### Standard Clock

```csharp
GameServerClock serverClock = new GameServerClock(50, 100);
serverClock.Tick += (frameNumber, deltaTime) =>
{
    // Game Logic
};
serverClock.Log += Console.WriteLine;
serverClock.Kill();
```

## CoRoutines

DeusaldSharp contains an implementation of Unity style coroutines logic. It can be used to execute part of the game
logic separated by server frames/seconds/conditions.

```csharp
CoRoCtrl.Reset();

IEnumerator<ICoData> TestMethod() // Definition of coroutine method
{
    // Game logic
    yield return CoRoCtrl.WaitForOneTick(); // Pause executing method for the next game server tick
    // Game logic
    yield return CoRoCtrl.WaitUntilDone(CoRoCtrl.RunCoRoutine(TestMethodTwo())); // Pause until other coroutine is done
    // Game logic
    yield return CoRoCtrl.WaitForSeconds(0.5f); // Pause for 0.5 second
    // Game logic
    yield return CoRoCtrl.WaitUntilTrue(() => pass); // Pause until condition is true
    // Game logic
}

CoRoCtrl.RunCoRoutine(TestMethod()); // Run coroutine
CoRoCtrl.Update(0.33f); // Update all coroutines with a given delta time
// You can start coRoutine with CoTag, CoTag is a tag to mark group of logically connected CoRoutines.
// The CoTag can be later used to pause or kill all CoRoutines marked with specific tag.
CoRoCtrl.RunCoRoutine(TestMethod2(), new CoTag(1)); 
// You can also save coHandle of coRoutine that let's you check and control specific coRoutine
ICoHandle coHandle = CoRoCtrl.RunCoRoutine(TestMethod2()); 
if (coHandle.IsAlive) coHandle.Kill();
```

## Extensions & Helpers

Group of useful extensions and helper methods.

```csharp
[Flags]
private enum TestFlags
{
    None = 0,
    A    = 1 << 0,
    B    = 1 << 1,
    C    = 1 << 2
}

private enum TestEnum
{
    A = 0,
    B = 1
}

// Testing if only one bit is set
TestFlags flags  = TestFlags.A;
TestFlags flags2 = TestFlags.A | TestFlags.C;
Assert.AreEqual(true, flags.IsSingleFlagOn());
Assert.AreEqual(false, flags2.IsSingleFlagOn());

// Getting random bit from those that are set
flags.GetRandomFlag((min, max) => new Random().Next(min, max));
flags.HasAnyFlag(TestFlags.A | TestFlags.C);

// Taking out arguments out of args
string[] args = { "10", "B" };
int      ten = args.TakeSimpleType(0, 0);
TestEnum b   = args.TakeEnum(1, TestEnum.A);

// Shuffle list
List<int> list = new List<int> { 1, 2, 3 };
list.Shuffle();

// ... etc.
```

## Math Utils

DeusaldSharp contains many math methods that can be useful when coding Game Server.

```csharp
// For example
4.Clamp(2, 3); // 3
MathUtils.Lerp(1, 10, 0.25f); // 3.25f
MathUtils.InverseLerp(1, 10, 7.75f); // 0.75f
7.451587f.RoundToDecimal(3); // 7.452f
0.1f.IsFloatZero(); // False
1.55f.AreFloatsEquals(1.55f); // True
7.MarkBit(2, false); // 5
// ... etc.
```

## Messages

Module that enables a possibility to listen for specific messages in a class.

```csharp
private class ExampleMsg
{
    public int    Int    { get; set; }
    public float  Float  { get; set; }
    public string String { get; set; }
    public object Object { get; set; }
}

// You can register class to listen for specific message type by using [MessageSlot] attribute
private class AttributeTest
{
    public int ReceivedMessages { get; private set; }
    
    public AttributeTest()
    {
        MsgCtrl.Register(this);
    }
    
    public void Unregister()
    {
        MsgCtrl.Unregister(this);
    }
    
    [MessageSlot]
    public void Receive(ExampleMsg message)
    {
        ++ReceivedMessages;
    }
}

// Allocate new instance of a message.
ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>(); // Messages instances are recycled from the pool.
msg.Int    = 10;
msg.Float  = 10.5f;
msg.String = "Test";
msg.Object = objectToReceive;
MsgCtrl.Send(msg); // Send messages to all classes that listen for this specific message

// Instead of using [MessageSlot] attribute you can also use Bind/Unbind methods
MsgCtrl.Bind<ExampleMsg>(Receive);
MsgCtrl.Unbind<ExampleMsg>(Receive);
```

## Spline 2D

DeusaldSharp contains class that lets you define and use 2d spline.

```csharp
// Arrange
Spline2D one = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
Spline2D two = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});

// Act
Vector2 positionOne   = one.InterpolateDistance(1.5f);
Vector2 positionTwo   = two.InterpolateDistance(1.5f);
Vector2 positionThree = one.InterpolateDistance(0.5f);
Vector2 positionFour  = two.InterpolateDistance(0.5f);
Vector2 positionFive  = one.InterpolateDistance(1.5f);
Vector2 positionSix   = two.InterpolateDistance(1.5f);

// Assert
Assert.Multiple(() =>
{
    Assert.AreEqual(new Vector2(0,           1.5015914f),  positionOne);
    Assert.AreEqual(new Vector2(1.0576555f,  1.0576555f),  positionTwo);
    Assert.AreEqual(new Vector2(0,           0.49840856f), positionThree);
    Assert.AreEqual(new Vector2(0.35238677f, 0.35238677f), positionFour);
    Assert.AreEqual(new Vector2(0,           1.5015914f),  positionFive);
    Assert.AreEqual(new Vector2(1.0576555f,  1.0576555f),  positionSix);
});
```

## Vector 2 & Vector 3

DeusaldSharp contains classes that implements Vector 2 and Vector 3 with all basic math connected to vectors.

```csharp
Vector2 one = new Vector2(1f,  1f);
Vector2 two = new Vector2(2f, -5f);
one.Normalized;
one.Negated;
one.Skew;
one.Set(2f, 3f);
Vector2.Add(one, two);
one.Cross(two);
one.Distance(two);
one.Dot(two);
one.Reflect(two);
Vector2.Clamp(one, oneMin, oneMax);
// etc.
```
## Typed Vector 2 & Vector 3
DeusaldSharp contains classes for typed Vector 2 and Vector 3.
```csharp
TVector2<int> one = new TVector2<int> {x = 10, y = 15};
TVector3<int> one = new TVector3<int> {x = 10, y = 15, z = 20};
```