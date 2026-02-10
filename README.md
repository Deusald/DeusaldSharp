# DeusaldSharp

Deusald Sharp is a library that contains useful functionalities when creating game in C#.

## Build

### [NuGet](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/v/DeusaldSharp?color=blue)](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/dt/DeusaldSharp)](https://www.nuget.org/packages/DeusaldSharp/)

### Unity Install

You can add this library to Unity project in 2 ways:

1. In package manager add library as git repository using format:
   `https://github.com/Deusald/DeusaldSharp.git?path=/UnityPackage.NetStandard2.1#vX.X.X`
2. Add package using Scoped Register: https://openupm.com/packages/com.deusald.deusaldsharp/

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

## Username Verificator

`UsernameVerificator` is a helper class for validating and cleaning usernames based on:
- minimum and maximum length,
- optional whitespace rule (no leading/trailing spaces),
- allowed character set (provided as a regex character-class).

### Example

```csharp
using DeusaldSharp;

UsernameVerificator verificator = new UsernameVerificator(
    minCharacters: 3,
    maxCharacters: 16,
    whitespaceRequirement: true,
    charactersRequirementRegex: @"[A-Za-z0-9 _]"
);

bool isValid = verificator.CheckUsernameRequirements("Adam_123");
string cleaned = verificator.CleanUsername("  Ad!am@@ 12_3  "); // "Adam 12_3"
```

## Glicko (Glicko-2 style rating updates)

`Glicko` is a helper for updating player ratings using a Glicko-2-like system.

### Data structure

```csharp
using DeusaldSharp;

GlickoData player = new GlickoData
{
    Rating     = Glicko.DEFAULT_RATING,
    Deviation  = Glicko.DEFAULT_DEVIATION,
    Volatility = Glicko.DEFAULT_VOLATILITY
};

// 1v1 update

GlickoData a = player;
GlickoData b = player;

// a wins (score = 1.0). draw = 0.5. loss = 0.0.
Glicko.Update(a, b, out GlickoData newA, out GlickoData newB, playerAScore: 1.0);

// Update vs multiple opponents

List<(GlickoData, double)> opponents = new()
{
    (b, 1.0), // win
    (b, 0.5), // draw
};

GlickoData updated = Glicko.Update(a, opponents);

// Win probability
double p = Glicko.GetWinProbability(a, b);

Decay (inactivity)
GlickoData decayed = Glicko.DecayPlayer(a, lastPlayedUtc, out bool didDecay);
```

## Binary serializers (BinaryWriter/BinaryReader extensions)

DeusaldSharp includes a set of `BinaryWriter` / `BinaryReader` extension methods for compact, allocation-friendly binary serialization of common types and their `List<T>` variants:

- Primitive lists: `byte`, `sbyte`, `bool`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `char`, `string`
- `Guid`, `DateTime`, `TimeSpan`, `Version` (and their list variants)
- Serializable enums (zero-boxing, explicit wire type) via `[SerializableEnum]`

### Primitive lists example

```csharp
using System.Collections.Generic;
using System.IO;
using DeusaldSharp;

using MemoryStream ms = new MemoryStream();
using BinaryWriter bw = new BinaryWriter(ms);

bw.Write(new List<int> { 1, 2, 3 });      // writes count + elements
bw.Write(new List<string> { "a", null }); // null -> "" (empty string)

bw.Flush();
ms.Position = 0;

using BinaryReader br = new BinaryReader(ms);
List<int>    ints    = br.ReadIntList();
List<string> strings = br.ReadStringList();
```

### Serializable enums
Annotate an enum with `[SerializableEnum]` to define the on-the-wire numeric type. This avoids boxing and keeps the binary format explicit and stable.

```csharp
using System.IO;
using DeusaldSharp;

[SerializableEnum(SerializableEnumType.Byte)]
public enum WeaponType : byte
{
    Sword = 1,
    Bow   = 2
}

using MemoryStream ms = new MemoryStream();
using BinaryWriter bw = new BinaryWriter(ms);

bw.WriteSerializableEnum(WeaponType.Sword);
bw.WriteSerializableEnumList(new System.Collections.Generic.List<WeaponType> { WeaponType.Sword, WeaponType.Bow });

bw.Flush();
ms.Position = 0;

using BinaryReader br = new BinaryReader(ms);
WeaponType one = br.ReadSerializableEnum<WeaponType>();

System.Collections.Generic.List<WeaponType> many = br.ReadSerializableEnumList<WeaponType>();
```
Notes
* List serialization format is always: int count followed by count elements.
* Write(List<string>) writes v ?? string.Empty.
* Serializable enums require the `[SerializableEnum]` attribute; otherwise reading/writing throws.

Below is a **single, detailed README section** you can copy-paste as-is.
It documents the **design goals, wire format, guarantees, edge cases, and usage patterns** of your Proto module, aligned with the final, fixed implementation and the expanded test suite.

---

## Proto module (ProtoMsg / ProtoModel / ProtoField)

The Proto module is a **lightweight, explicit, binary message system** designed for:
- deterministic serialization,
- schema evolution (forward/backward compatibility),
- zero reflection at runtime,
- full control over wire format,
- safe skipping of unknown fields.

It is intentionally **not** Protocol Buffers–compatible; instead, it is optimized for
game networking, save files, and internal tooling where stability and control matter more than compact varints.

---

### Core types

#### `ProtoMsg<TSelf>`
Base class for all messages.

Responsibilities:
- owns a static `ProtoModel<TSelf>` schema
- provides:
  - `byte[] Serialize()`
  - `static TSelf Deserialize(byte[])`

Each message type **must** assign `_model` in its static constructor.

```csharp
public sealed class MyMsg : ProtoMsg<MyMsg>
{
    public int X;

    static MyMsg()
    {
        _model = new ProtoModel<MyMsg>(
            ProtoField.Int<MyMsg>(1, static (ref MyMsg o) => ref o.X)
        );
    }
}
````

---

#### `ProtoModel<T>`

Defines the schema for a message.

* Holds an ordered list of `ProtoField<T>`
* Serializes each field independently as:

```
[ushort fieldId][int payloadLength][payloadBytes]
```

During deserialization:

* fields are read sequentially until end-of-stream
* unknown `fieldId`s are **skipped**
* duplicate `fieldId`s are allowed; **last value wins**

This guarantees:

* backward compatibility (new readers can read old data)
* forward compatibility (old readers skip new fields)

---

## Proto module (ProtoMsgBase / ProtoField)

The Proto module is a **lightweight, explicit, binary message format** with **source-generated** serialization.
It is designed for:

- deterministic serialization (stable bytes for the same state),
- schema evolution (forward/backward compatibility),
- **zero reflection at runtime**,
- fast skip of unknown fields,
- a wire format that is easy to debug.

It is intentionally **not** Protocol Buffers–compatible. It’s a pragmatic binary format aimed at game networking,
save files, and internal tooling where stability and control matter more than varint compactness.

---

### How it works

1. You create a **partial** message type that derives from `ProtoMsgBase`.
2. You mark instance fields with `[ProtoField(<ushort id>)]`.
3. The **DeusaldSharp.Analyzers** source generator emits `Serialize(BinaryWriter)` and `Deserialize(BinaryReader)`
   into a generated `.g.cs` file at build time.

> If a type derives from `ProtoMsgBase` but is not `partial`, you’ll get a build error.

---

### Quick start

```csharp
using DeusaldSharp;

public partial class LoginMsg : ProtoMsgBase
{
    [ProtoField(1)] public int    UserId;
    [ProtoField(2)] public string Name = "";
    [ProtoField(3)] public bool   IsGuest;
}

// Serialize
var msg  = new LoginMsg { UserId = 42, Name = "Ada", IsGuest = false };
byte[] bytes = msg.Serialize();

// Deserialize (instance)
var copy = new LoginMsg();
copy.Deserialize(bytes);

// Deserialize (generic helper)
LoginMsg copy2 = ProtoMsgBase.Deserialize<LoginMsg>(bytes);
```

---

### Wire format

Each field is serialized as a **length-delimited record**:

```
[ushort fieldId][int payloadLength][payloadBytes...]
```

Fields are written in ascending `fieldId` order (the generator sorts them).
Unknown `fieldId`s are skipped by reading `payloadLength` and seeking forward.

**Important:** serialization requires a **seekable** stream (e.g. `MemoryStream`) because the generator writes
`payloadLength` after the payload is written.

---

### Field IDs and compatibility

Rules enforced at compile time:

- `ProtoField` IDs must be **non-zero** `ushort`
- IDs must be **unique within a message** (duplicates are a generator error)

Compatibility behavior:

- **New reader reading old data:** missing fields stay at default (`0`, `false`, `null`, etc.)
- **Old reader reading new data:** unknown fields are skipped safely
- **Reordering fields:** safe; meaning is defined by ID, not order

---

### Supported field types

The generator supports:

**Primitives & string**
- `bool, byte, sbyte, short, ushort, int, uint, long, ulong, float, double, char, string`

**Special built-ins**
- `Guid, DateTime, TimeSpan, Version, System.Net.HttpStatusCode`

**Serializable enums**
- Any enum marked with `[SerializableEnum(...)]` (see the SerializableEnum module)

**Nullable value types**
- `Nullable<T>` where `T` is a supported primitive/special/serializable-enum

**Nested messages**
- Any non-nullable reference type deriving from `ProtoMsgBase`
- Nullable reference nested message (e.g. `ChildMsg?`) is supported as well

**Lists**
- `List<T>` where `T` is any supported primitive/special/serializable-enum
- `List<T>` where `T : ProtoMsgBase` (items may be `null`)
- Nullable list reference `List<T>?`

If a field type is not supported, you’ll get a build error pointing at the field.

---

### Nullability rules on the wire

**Nullable value types (`int?`, `Guid?`, etc.)**

```
[bool hasValue][value if hasValue]
```

**Nullable nested message (`ChildMsg?`)**

```
[bool hasValue][nested payload bytes...]
```

The nested message bytes are stored inside the field payload (the outer field already has a length).

**Object lists (`List<ChildMsg>`)**

```
[int count]
  repeat count times:
    [bool hasItem]
      false -> null element
      true  -> [int itemLen][item bytes...]
```

---

### Examples

#### 1) Serializable enum field

```csharp
using DeusaldSharp;

[SerializableEnum(SerializableEnumType.SByte)]
public enum PlayerState : sbyte
{
    Idle = 0,
    Playing = 1,
    Disconnected = -1
}

public partial class StateMsg : ProtoMsgBase
{
    [ProtoField(1)] public PlayerState State;
    [ProtoField(2)] public PlayerState? Previous;
}
```

#### 2) Nested messages + nullable nested messages

```csharp
using DeusaldSharp;

public partial class Vec2Msg : ProtoMsgBase
{
    [ProtoField(1)] public float X;
    [ProtoField(2)] public float Y;
}

public partial class SpawnMsg : ProtoMsgBase
{
    [ProtoField(1)] public int     EntityId;
    [ProtoField(2)] public Vec2Msg Position = new Vec2Msg();
    [ProtoField(3)] public Vec2Msg? Velocity; // nullable nested message
}
```

#### 3) Lists (including lists of messages)

```csharp
using System.Collections.Generic;
using DeusaldSharp;

public partial class InventoryMsg : ProtoMsgBase
{
    [ProtoField(1)] public List<int> ItemIds = new();
    [ProtoField(2)] public List<Vec2Msg> Checkpoints = new(); // items may be null if you put nulls in the list
    [ProtoField(3)] public List<string>? Tags;                // nullable list reference
}
```

---

### Error handling

- Unsupported field types / duplicate IDs / non-partial messages → **build-time errors** (generator diagnostics)
- Truncated/corrupt payloads → runtime exceptions from `BinaryReader` (fail-fast)
- Serializable-enum misuse (missing attribute, invalid underlying type) → runtime exceptions from the SerializableEnum helpers