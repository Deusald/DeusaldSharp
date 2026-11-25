# DeusaldSharp

Deusald Sharp is a library that contains useful functionalities when creating game in C#.

## Build

### [NuGet](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/v/DeusaldSharp?color=blue)](https://www.nuget.org/packages/DeusaldSharp/) [![NuGet](https://img.shields.io/nuget/dt/DeusaldSharp)](https://www.nuget.org/packages/DeusaldSharp/)

The library is split into modules, each published as its own NuGet package (and its own dll). All packages share one version.
Install `DeusaldSharp` to get everything, or pick only the modules you need:

| Package | Contents | Depends on |
|---|---|---|
| `DeusaldSharp` | Meta-package: references every module below | all modules |
| `DeusaldSharp.Core` | Extensions, BinaryWriter/BinaryReader serializers, Math utils, FlagsMemory, Helpers, Ansi, Username verificator | — |
| `DeusaldSharp.Clock` | Game Server Clock (Standard and Precision) | Core |
| `DeusaldSharp.CoRoutines` | Unity-style coroutines | Core |
| `DeusaldSharp.Vectors` | Vector2, Vector3, TVector2, TVector3 | Core |
| `DeusaldSharp.Splines` | 2D Spline | Core, Vectors |
| `DeusaldSharp.Proto` | Proto module (`ProtoMsgBase` / `ProtoField`) with the bundled source generator | Core |
| `DeusaldSharp.Messages` | Messages system | — |
| `DeusaldSharp.Glicko` | Glicko-2 style rating | — |
| `DeusaldSharp.Injector` | Dependency injection container | — |

All modules keep the single `DeusaldSharp` namespace, so switching between the meta-package and individual modules needs no code changes.

> The Proto source generator now ships inside `DeusaldSharp.Proto`. If your project references the old standalone
> `DeusaldSharp.Analyzers` package, remove it - otherwise the generator runs twice and produces duplicate members.

### Versioning

The version of every package, assembly and the Unity `package.json` lives in one place: `<Version>` in `Directory.Build.props`.
A Release build copies the module dlls into `UnityPackage.NetStandard2.1` and updates its `package.json` version.
To create the NuGet packages run `dotnet pack -c Release`.

### Unity Install

You can add this library to Unity project in 2 ways:

1. In package manager add library as git repository using format:
   `https://github.com/Deusald/DeusaldSharp.git?path=/UnityPackage.NetStandard2.1#vX.X.X`
   The Unity package contains a separate dll for every module (`DeusaldSharp.Core.dll`, `DeusaldSharp.Clock.dll`, ...).
2. Add package using Scoped Register: https://openupm.com/packages/com.deusald.deusaldsharp/

## Features

* Game Server Clock in two versions: Standard and Precise
* Clear C# Coroutines system similar to one that is build in Unity Engine
* Useful Enum/String/Task/List Extensions and Helpers
* Useful Math utils
* Messages system for sending messages to classes that can subscribe for specific messages
* 2D Spline class
* Vector2, Vector3 and Typed Vector2/Vector 3 classes with all classic vector math in it
* Simple dependency injection container with attribute based field/property injection

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

### Clock notes

- Both clocks are `IDisposable`; `Kill()` / `Dispose()` stop the clock thread and wait (up to 2 s) for it to finish, so no tick runs after they return.
- Subscribe to `TickException` to handle exceptions thrown by a `Tick` handler - the clock reports them and keeps ticking.
  Without a subscriber the exception is rethrown on the clock thread, which terminates the process (fail-fast).
- If a tick takes longer than the interval, the clock skips the missed frames instead of firing them back-to-back.

```csharp
serverClock.TickException += exception => logger.LogError(exception, "Tick failed");
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

### Fire-and-forget tasks

`Forget()` runs a task without awaiting it and without the "not awaited" warning. Exceptions never crash the process:
they go to the per-call handler, or to the global `TaskExtensions.UnhandledException` handler. Cancellation is ignored.

```csharp
// once at startup
DeusaldSharp.TaskExtensions.UnhandledException = e => Debug.LogException(e);  // Unity
DeusaldSharp.TaskExtensions.UnhandledException = e => logger.LogError(e, ""); // server

SaveAsync().Forget();                                   // uses the global handler
SendAsync().Forget(e => ShowError("Couldn't send", e)); // per-call handler
```

> Set the global handler in every project - when neither handler is set, exceptions from forgotten tasks are swallowed.
> `DeusaldSharp.TaskExtensions` is written fully qualified because `System.Threading.Tasks.TaskExtensions` has the same name.

### Reading untrusted binary data

All `BinaryReader` list/array readers validate the length prefix: a negative length, or one that cannot fit in the
remaining bytes of a seekable stream, throws `InvalidDataException` before anything is allocated. On non-seekable streams
memory grows only with the data actually received.

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

> `MsgCtrl` is static and **not thread-safe** - use it from a single thread (e.g. the game loop / Unity main thread).
> An exception thrown by a receiver propagates to the caller of `Send`; the bus keeps working and still-queued messages are delivered on the next `Send`.

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

// Win probability (uses both players' deviations)
double p = Glicko.GetWinProbability(a, b);
```

### Decay (inactivity)

Inactive players' rating deviation grows by one Glicko-2 rating period for every elapsed `period` after a deadline
(rating and volatility stay the same, deviation is capped at `DEFAULT_DEVIATION`). Reaching the deadline applies the
first period. Store the returned `nextDecayDeadline` - it keeps partially elapsed periods, so a player who logs in every
day decays gradually, not in bursts. Reset the deadline to `now + gracePeriod` when the player plays a game.

```csharp
// after a game: player.DecayDeadline = DateTime.UtcNow + TimeSpan.FromDays(14);

GlickoData decayed = Glicko.DecayPlayer(player.Glicko, player.DecayDeadline, DateTime.UtcNow, TimeSpan.FromDays(1),
                                        out long periodsApplied, out DateTime nextDecayDeadline);

if (periodsApplied > 0)
{
    player.Glicko        = decayed;
    player.DecayDeadline = nextDecayDeadline;
}
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
3. The source generator bundled in the **DeusaldSharp.Proto** package emits `Serialize(BinaryWriter)` and `Deserialize(BinaryReader)`
   into a generated `<Namespace>.<Type>.DeusaldSharpProto.cs` file at build time.

> If a type derives from `ProtoMsgBase` but is not `partial`, you’ll get a build error.

Supported message shapes:

- `public` and `internal` messages, messages split across several `partial` declarations.
- Nested message types - every containing type must be `partial` too (DSHP012).
- Inheritance - `[ProtoField]` fields of base messages are serialized by derived messages. Base fields must not be `private`
  (DSHP011) and field IDs must be unique across the whole inheritance chain (DSHP010).
- `[field: ProtoField(n)]` on auto-properties with a setter. `readonly` fields / get-only properties (DSHP008) and
  `static` / `const` fields (DSHP009) are rejected.

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
Unknown `fieldId`s are skipped using the validated `payloadLength` (see *Error handling*).

**Important:** serialization requires a **seekable** stream (e.g. `MemoryStream`) because the generator writes
`payloadLength` after the payload is written.

---

### Field IDs and compatibility

Rules enforced at compile time:

- `ProtoField` IDs must be **non-zero** `ushort`
- IDs must be **unique within a message** (duplicates are a generator error)

Compatibility behavior:

- **New reader reading old data:** missing fields keep the value of a freshly constructed message (field initializers are respected).
  This also holds when you reuse an instance: `msg.Deserialize(bytes)` resets absent fields first.
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

**Lists and arrays**
- `List<T>` / `T[]` where `T` is any supported primitive/special/serializable-enum
- `List<T>` / `T[]` where `T : ProtoMsgBase` (items may be `null`)
- Nullable list reference `List<T>?`

**Nullable reference types**
- `string?` and `Version?` are supported. A `null` string / `Version` field is simply not written, so the reader keeps its default.

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

- Unsupported field types / duplicate IDs / non-partial messages → **build-time errors** (generator diagnostics DSHP001-DSHP012)
- Malformed, truncated or malicious payloads → `System.IO.InvalidDataException` (the original exception, if any, is the `InnerException`).
  `Deserialize` is safe to call on untrusted network data:
  - field and item lengths and collection counts are validated against the remaining bytes **before** anything is allocated,
  - a negative or oversized length can't cause an infinite loop or a huge allocation,
  - nesting depth is limited by `ProtoMsgBase.MaxDepth` (default `ProtoMsgBase.DEFAULT_MAX_DEPTH` = 64; the root message is level 1),
    so a deeply nested payload can't overflow the stack.
- Serializable-enum misuse (missing attribute, invalid underlying type) → runtime exceptions from the SerializableEnum helpers

## Injector

`Injector` is a lightweight dependency injection container. You register objects in it (**bind**) and then let it fill
members marked with the `[Inject]` attribute (**inject**). It is deliberately minimal: no lifetimes, no scopes, no
automatic construction — you create the objects yourself and the container only wires them together.

The `[Inject]` attribute can be placed on **fields and properties**, both `public` and `private`, and injection is
resolved by the **exact declared type** of that member.

### Binding

```csharp
Injector injector = new Injector();

// Bind under the generic type argument
injector.Bind<IPlayerService>(new PlayerService());

// Bind under the concrete type (the generic argument is inferred)
injector.Bind(new Logger());

// Bind an object under every interface it implements at once
injector.BindAllInterfaces(new MatchmakingService()); // binds IMatchmaking, ITickable, ...

// Resolve manually
IPlayerService players = injector.Get<IPlayerService>();
```

The type used for the binding is the **generic type argument**, not the runtime type of the object. `Bind<IPlayerService>(new PlayerService())`
makes the object resolvable as `IPlayerService` only — asking for `PlayerService` will throw. The same applies to
`BindAllInterfaces`, which binds only the interfaces, never the concrete class. Binding the same type twice replaces the
previous binding.

Every `Injector` binds **itself** on construction, so `Get<Injector>()` works and `[Inject] Injector` members are filled
like any other dependency.

### Injecting

```csharp
public class GameLoop
{
    [Inject] public  IPlayerService Players { get; set; }
    [Inject] private Logger         _Logger;

    public void Tick() => _Logger.Log(Players.Count);
}

GameLoop loop = new GameLoop();
injector.Inject(loop); // Players and _Logger are now filled
```

`TriggerInjectOnBinders()` injects into everything that is bound in the container, which is the usual way to wire up a
whole system after all bindings are registered. Each object is injected only once, even when it is bound under several
types (for example through `BindAllInterfaces`):

```csharp
Injector injector = new Injector();

injector.Bind<IPlayerService>(new PlayerService());
injector.Bind(new Logger());
injector.BindAllInterfaces(new GameLoop());

injector.TriggerInjectOnBinders(); // every bound object gets its [Inject] members filled
```

### Notes

- Members **without** the `[Inject]` attribute are never touched.
- Injecting an object that has no `[Inject]` members is a no-op, not an error.
- Requesting or injecting a type that was never bound throws `KeyNotFoundException` — bind everything before calling
  `Inject` / `TriggerInjectOnBinders`.
- Resolution is by exact declared type, so a member declared as `PlayerService` is not satisfied by a
  `Bind<IPlayerService>` binding.

## Ansi Console Colors

`Ansi` is a static class providing ANSI escape code constants and helpers for colorizing console output. It supports three levels of color control:

- **Named constants** — 8 standard foreground and background colors, plus bright foreground variants, available as plain `string` constants with zero allocation.
- **True-color methods** — `Fg(r, g, b)` and `Bg(r, g, b)` for full 24-bit RGB control, accepted by any terminal that supports ANSI true color.
- **`RESET`** — a constant that clears all active formatting. Always append it after colored text to prevent color bleeding into subsequent output.

> **Terminal support:** Windows Terminal, macOS Terminal, and most Linux terminals support ANSI escape codes out of the box. The legacy Windows Console Host (cmd.exe) requires virtual terminal processing to be enabled — .NET 5+ enables it automatically; on older runtimes you may need to set it manually via `SetConsoleMode`.

### Example

```csharp
// Named foreground color
Console.WriteLine(Ansi.GREEN + "Success!" + Ansi.RESET);

// Named background + foreground combination
Console.WriteLine(Ansi.BG_RED + Ansi.WHITE + " ERROR " + Ansi.RESET + " Something went wrong.");

// Bright variant
Console.WriteLine(Ansi.BRIGHT_BLUE + "Info message" + Ansi.RESET);

// 24-bit true color — foreground
Console.WriteLine(Ansi.Fg(255, 165, 0) + "Orange text" + Ansi.RESET);

// 24-bit true color — background via RGB
Console.WriteLine(Ansi.Bg(30, 30, 30) + Ansi.WHITE + " Dark background " + Ansi.RESET);

// 24-bit true color — background via System.Drawing.Color
Console.WriteLine(Ansi.Bg(Color.CornflowerBlue) + " CornflowerBlue background " + Ansi.RESET);

// Building a colored table row
string row = Ansi.Bg(50, 50, 80) + Ansi.BRIGHT_WHITE
           + $" {"Player",-12} {"Score",6} "
           + Ansi.RESET;
Console.WriteLine(row);
```

### Available constants

| Constant | Effect |
|---|---|
| `RESET` | Clears all formatting |
| `BLACK` `RED` `GREEN` `YELLOW` `BLUE` `MAGENTA` `CYAN` `WHITE` | Standard foreground colors |
| `BG_BLACK` `BG_RED` `BG_GREEN` `BG_YELLOW` `BG_BLUE` `BG_MAGENTA` `BG_CYAN` `BG_WHITE` | Standard background colors |
| `BRIGHT_RED` `BRIGHT_GREEN` `BRIGHT_BLUE` `BRIGHT_WHITE` | Bright foreground variants |
| `Fg(r, g, b)` | 24-bit true-color foreground |
| `Bg(r, g, b)` / `Bg(Color)` | 24-bit true-color background |