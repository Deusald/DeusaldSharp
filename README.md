# DeusaldSharp

[![NuGet](https://img.shields.io/nuget/v/DeusaldSharp?color=blue)](https://www.nuget.org/packages/DeusaldSharp/)
[![NuGet](https://img.shields.io/nuget/dt/DeusaldSharp)](https://www.nuget.org/packages/DeusaldSharp/)

DeusaldSharp is a set of C# libraries for game development - game servers and Unity clients alike: fixed-tick server
clocks, Unity-style coroutines, a message bus, a dependency injector, Glicko-2 ratings, vectors and splines, a safe
binary serialization layer and a source-generated binary message format (Proto).

Targets **.NET Standard 2.1** (Unity, Mono, .NET Core 3+) and **.NET 10**. All modules use the single `DeusaldSharp` namespace.

- [Installation](#installation)
- [Modules](#modules)
- Core: [Extensions](#extensions) · [Binary serialization](#binary-serialization) · [Math utils](#math-utils) ·
  [Flags memory](#flags-memory) · [Helpers](#helpers) · [Username verificator](#username-verificator) · [Ansi console colors](#ansi-console-colors) · [Enums](#enums)
- [Vectors](#vectors) · [Splines](#splines) · [Clock](#clock) · [CoRoutines](#coroutines) · [Messages](#messages) ·
  [Injector](#injector) · [Glicko](#glicko) · [Proto](#proto)
- [Upgrading to 11.0](#upgrading-to-110) · [Building](#building) · [Patch notes](PatchNotes)

---

## Installation

### NuGet

Install the `DeusaldSharp` meta-package to get every module, or reference only the modules you need:

```
dotnet add package DeusaldSharp            # everything
dotnet add package DeusaldSharp.Proto      # or just the modules you use
```

### GitHub Packages

Every release is also published to GitHub Packages, where it is available right after the release build (NuGet.org
indexing can take a while). GitHub Packages requires authentication even for public packages: create a
[personal access token](https://github.com/settings/tokens) with the `read:packages` scope and add the source once:

```
dotnet nuget add source https://nuget.pkg.github.com/Deusald/index.json --name deusald-github --username YOUR_GITHUB_USERNAME --password YOUR_TOKEN --store-password-in-clear-text
```

or per repository in `nuget.config` (keep the token in an environment variable, not in the file):

```xml
<configuration>
  <packageSources>
    <add key="deusald-github" value="https://nuget.pkg.github.com/Deusald/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <deusald-github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="%GITHUB_PACKAGES_TOKEN%" />
    </deusald-github>
  </packageSourceCredentials>
</configuration>
```

### Unity

1. Package Manager → *Add package from git URL*:
   `https://github.com/Deusald/DeusaldSharp.git?path=/UnityPackage.NetStandard2.1#vX.X.X`
2. Or via scoped registry: https://openupm.com/packages/com.deusald.deusaldsharp/

The Unity package contains one dll per module (`DeusaldSharp.Core.dll`, `DeusaldSharp.Clock.dll`, ...).
The Proto source generator is not part of the Unity package.

---

## Modules

Every module is a separate NuGet package and dll. All packages share one version.

| Package | Contents | Depends on |
|---|---|---|
| `DeusaldSharp` | Meta-package referencing every module below | all modules |
| `DeusaldSharp.Core` | [Extensions](#extensions), [binary serialization](#binary-serialization), [math utils](#math-utils), [flags memory](#flags-memory), [helpers](#helpers), [username verificator](#username-verificator), [Ansi colors](#ansi-console-colors), [enums](#enums) | — |
| `DeusaldSharp.Vectors` | [`Vector2`, `Vector3`, `TVector2<T>`, `TVector3<T>`](#vectors) | Core |
| `DeusaldSharp.Splines` | [`Spline2D`](#splines) | Core, Vectors |
| `DeusaldSharp.Clock` | [`GameServerClock`, `PrecisionClock`](#clock) | Core |
| `DeusaldSharp.CoRoutines` | [Unity-style coroutines](#coroutines) | Core |
| `DeusaldSharp.Messages` | [Message bus with pooled messages](#messages) | — |
| `DeusaldSharp.Injector` | [Attribute-based dependency injection](#injector) | — |
| `DeusaldSharp.Glicko` | [Glicko-2 rating, win probability and decay](#glicko) | — |
| `DeusaldSharp.Proto` | [Binary messages with a bundled source generator](#proto) | Core |

---

## Extensions

`DeusaldSharp.Core`

### Enum flags

Allocation-free flag operations for any enum (no boxing, any underlying type).

```csharp
[Flags]
public enum Abilities { None = 0, Jump = 1 << 0, Dash = 1 << 1, Fly = 1 << 2 }

Abilities abilities = Abilities.Jump | Abilities.Dash;

abilities.IsSingleFlagOn();                           // false
abilities.CountFlags();                               // 2
abilities.HasAnyFlag(Abilities.Dash | Abilities.Fly); // true
abilities.HasAllFlags(Abilities.Dash | Abilities.Fly);// false
abilities.WithFlags(Abilities.Fly);                   // Jump, Dash, Fly
abilities.WithoutFlags(Abilities.Jump);               // Dash
Abilities.None.IsNoFlagOn();                          // true

foreach (Abilities ability in abilities.IterateSetFlags()) { } // Jump, Dash - struct enumerator, no allocation

EnumExtensions.IterateAllValues<Abilities>();         // None, Jump, Dash, Fly (cached)
EnumExtensions.IterateAllSingleFlags<Abilities>();    // Jump, Dash, Fly (cached)

abilities.GetRandomFlag(random.Next);                 // Jump or Dash
abilities.ToBits();                                   // 3
EnumExtensions.FromBits<Abilities>(4);                // Fly

// Read a custom attribute placed on an enum member
DescriptionAttribute description = myEnumValue.GetCustomExtension<DescriptionAttribute>();
```

### Strings

```csharp
"PlayerMaxHealth".ToSnakeCase();     // "player_max_health"
"PlayerMaxHealth".SplitPascalCase(); // "Player Max Health"
"player".FirstToUpper();             // "Player"
"   ".IsBlank();                     // true (also IsNullOrEmpty / IsNullOrWhiteSpace)
nullableText.OrEmpty();              // "" when null
"Hello".Base64Encode();              // "SGVsbG8="
"SGVsbG8=".Base64Decode();           // "Hello"
"DB_HOST".GetEnvironmentVariable();  // value or null

// Passwords: PBKDF2-SHA512, 100k iterations, random salt, constant-time comparison
string hash = "secret123".HashPassword();
"secret123".CompareHashedPasswords(hash); // true

// HMAC-SHA256 (UTF-8), lowercase hex
"what do ya want for nothing?".ComputeHmacSha256Hex("Jefe"); // "5bdcc146bf60754e..."
```

### Byte arrays, lists and bits

```csharp
byte[] bytes = { 0xDE, 0xAD, 0xBE, 0xEF };
bytes.ByteArrayToHexString();        // "deadbeef"
"deadbeef".HexStringToByteArray();   // { 0xDE, 0xAD, 0xBE, 0xEF } - FormatException on invalid input
bytes.ByteArrayToBase64String();     // "3q2+7w=="
"3q2+7w==".Base64StringToByteArray();

list.Shuffle();                      // unbiased Fisher-Yates using a cryptographic RNG

ulong mask = 0b1011_0000;
BitOps.TrailingZeroCount(mask);      // 4
BitOps.PopCount(mask);               // 3
ulong rest = mask.PopLowestSetBit(out int index, out ulong bit); // rest = 0b1010_0000, index = 4, bit = 0b1_0000
```

### Fire-and-forget tasks

`Forget()` runs a task without awaiting it (and without the "not awaited" warning). Exceptions **never crash the
process**: they go to the per-call handler, or to the global `TaskExtensions.UnhandledException`. Cancellation is ignored.

```csharp
// once at startup
DeusaldSharp.TaskExtensions.UnhandledException = e => Debug.LogException(e);        // Unity
DeusaldSharp.TaskExtensions.UnhandledException = e => logger.LogError(e, "Forget"); // server

SaveAsync().Forget();                                   // global handler
SendAsync().Forget(e => ShowError("Couldn't send", e)); // per-call handler
```

> Set the global handler in every project - without any handler, exceptions from forgotten tasks are swallowed.
> Write `DeusaldSharp.TaskExtensions` fully qualified: `System.Threading.Tasks.TaskExtensions` has the same name.

### Zip

```csharp
using ZipArchive archive = ZipFile.Open("backup.zip", ZipArchiveMode.Create);
archive.CreateEntryFromAny(@"C:\Game\Saves", "Backup"); // file or whole directory, recursively
// entries: "Backup/Saves/slot1.json", "Backup/Saves/Empty/" - '/' separators, empty directories kept, symlinks skipped
```

---

## Binary serialization

`DeusaldSharp.Core`

`BinaryWriter` / `BinaryReader` extensions for lists, arrays and common types:

- primitive `List<T>` and `T[]`: `byte`, `sbyte`, `bool`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `char`, `string`
- `Guid`, `DateTime`, `TimeSpan`, `Version`, `HttpStatusCode` and their lists / arrays
- serializable enums with an explicit wire type

```csharp
using MemoryStream ms = new MemoryStream();
using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
{
    writer.Write(new List<int> { 1, 2, 3 });   // [int count][elements]
    writer.Write(new List<string> { "a", null }); // null is written as ""
    writer.WriteArray(new[] { 1.5f, 2.5f });   // arrays use WriteArray
    writer.Write(Guid.NewGuid());
    writer.Write(TimeSpan.FromSeconds(1.5));  // int64 ticks - exact
    writer.Write(new Version(1, 2));          // all four components
}

ms.Position = 0;
using BinaryReader reader = new BinaryReader(ms);
List<int>    ints    = reader.ReadIntList();    // 1, 2, 3
List<string> strings = reader.ReadStringList(); // "a", ""
float[]      floats  = reader.ReadFloatArray(); // 1.5, 2.5
Guid         id      = reader.ReadGuid();
TimeSpan     time    = reader.ReadTimeSpan();   // 00:00:01.5
Version      version = reader.ReadVersion();    // 1.2

// Helpers for byte[] round-trips
byte[] blob = Helpers.SerializeByBinaryWriter(w => { w.Write(42); w.Write("hi"); });
Helpers.DeserializeFromBinaryReader(blob, r => { int number = r.ReadInt32(); string text = r.ReadString(); });
```

### Serializable enums

`[SerializableEnum]` fixes the on-the-wire integer type of an enum, so the binary format stays explicit and stable
(no boxing). Values that don't fit the wire type throw; unsigned enums are never sign-extended.

```csharp
[SerializableEnum(SerializableEnumType.Byte)]
public enum WeaponType : byte { Sword = 1, Bow = 2 }

writer.WriteSerializableEnum(WeaponType.Bow);
writer.WriteSerializableEnumList(new List<WeaponType> { WeaponType.Sword, WeaponType.Bow });
writer.WriteSerializableEnumArray(new[] { WeaponType.Bow });

WeaponType       one  = reader.ReadSerializableEnum<WeaponType>();
List<WeaponType> many = reader.ReadSerializableEnumList<WeaponType>();
```

Wire types: `Byte`, `SByte`, `Short`, `UShort`, `Int`, `UInt`, `Long`, `ULong`.

### Untrusted data

The readers are safe to use on data from the network. Every list / array length is validated **before** allocating:
a negative length, or one that can't fit in the remaining bytes of a seekable stream, throws `InvalidDataException`.
On non-seekable streams memory grows only with the bytes actually received.

```csharp
new BinaryReader(new MemoryStream(new byte[] { 0xFF, 0xFF, 0xFF, 0x7F })).ReadByteArray();
// InvalidDataException - instead of allocating 2 GB
```

---

## Math utils

`DeusaldSharp.Core`

```csharp
4.Clamp(2, 3);                         // 3 - all numeric types; throws when min > max
MathUtils.Lerp(1f, 10f, 0.25f);        // 3.25
MathUtils.InverseLerp(1f, 10f, 7.75f); // 0.75 (returns 0 when a == b)
7.451587f.RoundToDecimal(3);           // 7.452
(-1).MathMod(5);                       // 4 - always non-negative
(-3).IsOdd();                          // true

// Float comparison with tolerance MathUtils.EPSILON (1e-6, relative above magnitude 1)
(0.1f + 0.2f).AreFloatsEquals(0.3f);   // true
1e-7f.IsFloatZero();                   // true

// Bits (all integer types)
0b0111.MarkBit(0b0010, false);         // 0b0101 - set / clear the bits of a mask
0b1011.NumberOfSetBits();              // 3
8.IsSingleBitOn();                     // true
0b1010.HasAnyBitOn(0b0010);            // true
0b1010.HasAllBitsOn(0b0110);           // false

// Constants
MathUtils.DEG_TO_RAD; MathUtils.RAD_TO_DEG; MathUtils.SEC_TO_MILLISECONDS; MathUtils.MILLISECONDS_TO_SECONDS;
MathUtils.MIN_TO_SEC; MathUtils.HOURS_TO_MIN; MathUtils.DAYS_TO_HOURS;
```

---

## Flags memory

`DeusaldSharp.Core`

Immutable bit sets: `ByteFlagsMemory` (8), `UshortFlagsMemory` (16), `UintFlagsMemory` (32), `UlongFlagsMemory` (64).
Handy for unlocked levels, achievements, per-slot states. An out-of-range index throws `ArgumentOutOfRangeException`.

```csharp
UintFlagsMemory unlocked = UintFlagsMemory.Empty.Set(0).Set(5);

unlocked.IsSet(5);                 // true
unlocked.FlagsSetCount;            // 2
unlocked.Clear(0).Raw;             // 32
unlocked.SetTo(5, false);          // set or clear by bool
unlocked.IsEmpty; unlocked.IsFull;
UintFlagsMemory.OneUpToIndex(4);   // bits 0..3 set (Raw = 15)
new UintFlagsMemory(raw);          // from a stored value

ByteFlagsMemory.Empty.Set(1).Set(3).ToString(); // "[ByteFlagsMemory 00001010 count=2]"
```

---

## Helpers

`DeusaldSharp.Core`

### Command line arguments

Read typed values from `string[] args` with defaults. Missing / negative indexes and unparsable values return the default;
parsing is culture-invariant.

```csharp
string[] args = { "7777", "Warn", "eu-west", "Adam" };

int      port     = args.TakeSimpleType(0, 8080);                                       // 7777
LogLevel logLevel = args.TakeEnum(1, LogLevel.Info);                                    // Warn
string   region   = args.TakeFromList(2, new List<string> { "eu-west", "us-east" }, "us-east"); // "eu-west"
string   bot      = args.TakeUsername(3, "Guest");                                      // "Adam"
string   other    = args.TakeString(9, "default");                                      // "default"
```

`TakeEnum` rejects numeric values that aren't defined in the enum (`[Flags]` enums accept any combination).

### Random

```csharp
bool coinFlip = Helpers.GetRandomBool(); // cryptographic RNG
```

---

## Username verificator

`DeusaldSharp.Core`

Validates and cleans usernames by length, leading / trailing whitespace and an allowed character set.
The character set is the **content** of a regex character class (without the brackets).

```csharp
UsernameVerificator verificator = new UsernameVerificator(
    minCharacters: 3,
    maxCharacters: 16,
    whitespaceRequirement: true,         // no leading / trailing spaces
    charactersRequirementRegex: "A-Za-z0-9 _");

verificator.CheckUsernameRequirements("Adam_123");   // true
verificator.CheckUsernameRequirements(" Adam");      // false - leading space
verificator.CheckUsernameRequirements("Ad!am");      // false - '!' not allowed
verificator.CheckUsernameRequirements(null);         // false
verificator.CleanUsername("  Ad!am@@ 12_3  ");       // "Adam 12_3"
```

Individual checks are available too: `CheckUsernameLengthRequirement`, `CheckUsernameWhitespaceRequirement`, `CheckCharactersRequirement`.

> Keep the character set ASCII (e.g. `A-Za-z0-9 _`). Unicode classes such as `\w` or `\p{L}` allow look-alike names
> (Cyrillic "а" vs Latin "a").

---

## Ansi console colors

`DeusaldSharp.Core`

ANSI escape codes for colored console output: named colors as `string` constants and 24-bit true color.

```csharp
Console.WriteLine(Ansi.GREEN + "Success!" + Ansi.RESET);
Console.WriteLine(Ansi.BG_RED + Ansi.WHITE + " ERROR " + Ansi.RESET + " Something went wrong.");
Console.WriteLine(Ansi.BRIGHT_BLUE + "Info" + Ansi.RESET);
Console.WriteLine(Ansi.Fg(255, 165, 0) + "Orange text" + Ansi.RESET);
Console.WriteLine(Ansi.Bg(30, 30, 30) + Ansi.WHITE + " Dark background " + Ansi.RESET);
Console.WriteLine(Ansi.Bg(Color.CornflowerBlue) + " System.Drawing.Color " + Ansi.RESET);
```

| Member | Effect |
|---|---|
| `RESET` | Clears all formatting - always append it after colored text |
| `BLACK` `RED` `GREEN` `YELLOW` `BLUE` `MAGENTA` `CYAN` `WHITE` | Foreground colors |
| `BG_BLACK` `BG_RED` `BG_GREEN` `BG_YELLOW` `BG_BLUE` `BG_MAGENTA` `BG_CYAN` `BG_WHITE` | Background colors |
| `BRIGHT_RED` `BRIGHT_GREEN` `BRIGHT_BLUE` `BRIGHT_WHITE` | Bright foreground colors |
| `Fg(r, g, b)` | True-color foreground |
| `Bg(r, g, b)` / `Bg(Color)` | True-color background |

> Windows Terminal, macOS and Linux terminals support ANSI codes out of the box; on the legacy Windows console .NET 5+
> enables them automatically.

---

## Enums

`DeusaldSharp.Core`

- **`LogLevel`** - `Off`, `Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`.
- **`Platforms`** - `[Flags]` serializable enum: `Windows`, `MacOS`, `Linux`, `iOS`, `Android`, `Xbox`, `PlayStation`,
  `NintendoSwitch` and the groups `Desktop`, `Mobile`, `Console`, `All`.
- **`DeusaldSharp.Version`** - version of the library (`11.0.2`).

```csharp
Platforms.Desktop.HasAnyFlag(Platforms.Linux); // true
```

---

## Vectors

`DeusaldSharp.Vectors`

`Vector2` and `Vector3` (`float`) with the classic vector math, plus generic `TVector2<T>` / `TVector3<T>` for grids,
chunks or any struct type.

```csharp
Vector2 a = new Vector2(3f, 4f);
Vector2 b = Vector2.Right;            // also Zero, One, Up, Down, Left

a.Magnitude;                          // 5 (also SqrMagnitude, Length(), LengthSquared())
a.Normalized;                         // (0.6, 0.8) - a.Normalize() normalizes in place
a.Negated;                            // (-3, -4)
a.Skew;                               // (-4, 3)
a.Dot(b);                             // 3
a.Cross(b);                           // -4
a.Distance(b);                        // 4.47 (also DistanceSquared)
new Vector2(1f, -1f).Reflect(Vector2.Up);   // (1, 1)
Vector2.Lerp(Vector2.Zero, a, 0.5f);        // (1.5, 2)
Vector2.Clamp(a, Vector2.Zero, Vector2.One * 2f); // (2, 2)
Vector2.MoveTowards(Vector2.Zero, a, 2.5f); // (1.5, 2)
Vector2.Rotate(MathF.PI / 2f, b);           // (0, 1) - radians; RotateInverse rotates the other way
Vector2.Min(a, b); Vector2.Max(a, b); Vector2.Abs(a);
a + b * 2f;                           // (5, 4) - operators + - * /
a[1];                                 // 4 - indexer
a.IsValid;                            // false for NaN / infinity

Vector3 v = new Vector3(1f, 2f, 2f);
v.Magnitude;                          // 3
Vector3.Cross(Vector3.UnitX, Vector3.UnitY); // (0, 0, 1)
Vector3.Forward;                      // (0, 0, -1) - right-handed; also Up, Down, Left, Right, Backward
Vector2 flat = v;                     // implicit Vector3 -> Vector2

TVector2<int> cell  = new TVector2<int>(10, 15);
TVector3<int> chunk = new TVector3<int>(1, 2, 3);
Dictionary<TVector3<int>, Chunk> world = new(); // value equality and hashing on all components
```

> `==` / `!=` compare with the float tolerance of `MathUtils.AreFloatsEquals` (like Unity), while `Equals` / `GetHashCode`
> are exact so vectors work as dictionary keys.

---

## Splines

`DeusaldSharp.Splines`

`Spline2D` is a cardinal (Hermite) spline through 2D control points - paths for AI, projectiles, cameras or tracks.

```csharp
Spline2D path = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0) },
                             closed: false, curvature: 0.5f, lengthSamplesPerSegment: 5);

path.Length;                    // 2.88 - approximate curve length
path.Interpolate(0.5f);         // (1, 1) - position at 50% of the whole curve
path.InterpolateDistance(1f);   // position 1 unit along the curve
path.Derivative(0.5f);          // (1, 0) - tangent (also DerivativeDistance)
path.Interpolate(1, 0.5f);      // halfway between point 1 and point 2
path.DistanceToLinearT(1f);     // 0.335 - distance -> t
path.DistanceAtPoint(2);        // distance along the curve at control point 2

// Editing
path.AddPoint(new Vector2(3, 1));
path.AddPointScroll(new Vector2(4, 0)); // add at the end, drop the first point (moving windows)
path.InsertPoint(1, new Vector2(0.5f, 0.5f));
path.SetPoint(0, Vector2.Zero);         // also path[0] = ...
path.ReplacePoints(newPoints);
path.RemovePoint(1);
path.Clear();
```

- Open splines clamp `t` to `[0, 1]`; closed splines (`IsClosed`) wrap it, so `Interpolate(1.25f) == Interpolate(0.25f)`.
- An empty spline interpolates to `Vector2.Zero`, a single-point spline to that point.
- `Curvature` and `LengthSamplesPerSegment` (≥ 1, length accuracy) can be changed at any time.

---

## Clock

`DeusaldSharp.Clock`

Fixed-tick clocks that run game server logic on their own thread.

| Clock | Tick event | Notes |
|---|---|---|
| `GameServerClock(ticksPerSecond, logEveryXFrame)` | `(ulong frameNumber, double deltaTime)` | Light on CPU. `Log` reports average / longest tick every `logEveryXFrame` frames |
| `PrecisionClock(ticksPerSecond)` | `(ulong frameNumber)` | Tighter timing, uses more CPU. `TooLongFrame` fires when a tick overruns |

```csharp
using GameServerClock clock = new GameServerClock(ticksPerSecond: 50, logEveryXFrame: 250);

clock.Tick += (frameNumber, deltaTime) =>
{
    world.Update(deltaTime);
};
clock.Log           += message => logger.LogInformation(message); // "Frame 250. Average frame length ... ms ..."
clock.TickException += exception => logger.LogError(exception, "Tick failed");

// ... later
clock.Kill(); // or Dispose()
```

```csharp
PrecisionClock clock = new PrecisionClock(60);
clock.Tick         += frameNumber => Simulate();
clock.TooLongFrame += () => Console.WriteLine("Too long frame");
```

- The clock starts ticking when it is created; `IsAlive` tells whether it still runs.
- The tick rate is exact (scheduled in `Stopwatch` ticks). When a tick takes longer than the interval, missed frames are
  **skipped** instead of fired back-to-back.
- `TickException`: with a subscriber, exceptions from `Tick` handlers are reported and the clock keeps ticking. Without
  one, the exception is rethrown on the clock thread and terminates the process (fail-fast).
- `Kill()` / `Dispose()` stop the clock and wait (up to 2 s) for the running tick, so no tick runs after they return.

---

## CoRoutines

`DeusaldSharp.CoRoutines`

Unity-style coroutines for any game loop: write sequential logic that waits for ticks, seconds, conditions or other
coroutines. Create a `CoRoCtrl` and call `Update(deltaTime)` once per frame, after the rest of your frame logic.

```csharp
CoRoCtrl coRoCtrl = new CoRoCtrl();

IEnumerator<ICoData> OpenDoor()
{
    PlayAnimation("door_open");
    yield return coRoCtrl.WaitForSeconds(0.5f);       // wait 0.5 s of game time
    _DoorOpen = true;
}

IEnumerator<ICoData> StartLevel()
{
    SpawnPlayer();
    yield return coRoCtrl.WaitForOneTick();           // next Update
    yield return coRoCtrl.WaitUntilDone(OpenDoor());  // run another coroutine and wait for it
    yield return coRoCtrl.WaitUntilTrue(() => _PlayerReady); // also WaitUntilFalse
    StartTimer();
}

ICoHandle level = coRoCtrl.RunCoRoutine(StartLevel());

// game loop
coRoCtrl.Update(deltaTime);
```

### Controlling coroutines

```csharp
ICoHandle handle = coRoCtrl.RunCoRoutine(EnemyAi(), new CoTag(7), coMask: 0b0101);

handle.IsAlive;           // false when finished or killed
handle.IsPaused = true;   // pause / resume
handle.Kill();

// Groups: by tag, or by bit mask (MaskType.Any / MaskType.All)
coRoCtrl.PauseCoRoutines(new CoTag(7));
coRoCtrl.UnpauseCoRoutines(new CoTag(7));
coRoCtrl.KillCoRoutines(0b0100, CoRoCtrl.MaskType.Any);

// Inside a coroutine
ICoHandle self = coRoCtrl.GetCurrentCoHandle();

coRoCtrl.Reset();         // kills everything
```

- Killing a coroutine stops it immediately - even from inside its own body - and runs its `finally` / `using` blocks.
- An exception thrown in a coroutine kills that coroutine and propagates to the caller of `Update`; the other
  coroutines keep running.
- Repeated `WaitForSeconds` don't drift: overshoot from one wait is carried into the next.
- `RunCoRoutine` executes the coroutine up to its first `yield` immediately; the rest continues in the following `Update` calls.

---

## Messages

`DeusaldSharp.Messages`

A static message bus: classes subscribe to message types and receive every message sent of that type.
Message instances are pooled - allocate them from the bus and they are recycled after delivery.

```csharp
public class DamageMsg
{
    public int    Amount { get; set; }
    public Entity Target { get; set; }
}

// Subscribe with [MessageSlot] methods (any visibility, base classes included)
public class HealthBar
{
    public HealthBar()       => MsgCtrl.Register(this);
    public void Dispose()    => MsgCtrl.Unregister(this);

    [MessageSlot]
    private void OnDamage(DamageMsg message) { _Hp -= message.Amount; }
}

// ... or bind delegates
MsgCtrl.Bind<DamageMsg>(OnDamage);
MsgCtrl.Unbind<DamageMsg>(OnDamage);

// Send
DamageMsg damage = MsgCtrl.Allocate<DamageMsg>(); // from the pool
damage.Amount = 25;
MsgCtrl.Send(damage);                             // delivered, then reset and returned to the pool
```

- **Priority**: `[MessageSlot(priority)]` / `Bind(method, priority)` - receivers with **lower** values are called first.
- **Consume**: call `MsgCtrl.Consume()` inside a receiver to stop delivery to the remaining receivers.
- `MsgCtrl.Deallocate(message)` returns a message that won't be sent; `MsgCtrl.Clear()` removes all receivers.
- Messages sent from inside a receiver are queued and delivered after the current one.
- Pooled messages are reset between uses (writable public properties and fields go back to their defaults).

> `MsgCtrl` is **not thread-safe** - use it from one thread (game loop / Unity main thread). An exception thrown by a
> receiver propagates to the caller of `Send`; the bus keeps working and still-queued messages are delivered on the next `Send`.

---

## Injector

`DeusaldSharp.Injector`

A minimal dependency injection container: you create the objects, **bind** them, and the injector fills members marked
with `[Inject]`. No lifetimes, scopes or automatic construction.

```csharp
Injector injector = new Injector();

injector.Bind<IPlayerService>(new PlayerService()); // bound as IPlayerService
injector.Bind(new GameLogger());                    // bound as GameLogger
injector.BindAllInterfaces(new MatchmakingService()); // bound as every interface it implements

IPlayerService players = injector.Get<IPlayerService>();
```

```csharp
public class GameLoop
{
    [Inject] public  IPlayerService Players { get; set; }
    [Inject] private GameLogger     _Logger;
}

GameLoop loop = new GameLoop().Inject(injector); // or injector.Inject(loop)

injector.TriggerInjectOnBinders(); // inject into every bound object (each object once)
```

- `[Inject]` works on fields and properties of any visibility, including private members of base classes.
- Resolution uses the exact declared member type and the type used when binding (`Bind<IPlayerService>` is not
  resolvable as `PlayerService`). Binding a type again replaces it.
- Every `Injector` binds itself, so `[Inject] Injector` members work.
- Getting or injecting a type that was never bound throws `KeyNotFoundException`.

---

## Glicko

`DeusaldSharp.Glicko`

[Glicko-2](http://www.glicko.net/glicko/glicko2.pdf) player ratings: rating updates, win probability for matchmaking
and rating-deviation decay for inactive players.

```csharp
GlickoData player = new GlickoData
{
    Rating     = Glicko.DEFAULT_RATING,     // 1500
    Deviation  = Glicko.DEFAULT_DEVIATION,  // 350
    Volatility = Glicko.DEFAULT_VOLATILITY  // 0.06
};
```

### Updating ratings

```csharp
GlickoData a = new GlickoData { Rating = 1500, Deviation = 200, Volatility = 0.06 };
GlickoData b = new GlickoData { Rating = 1400, Deviation = 30,  Volatility = 0.06 };

// 1v1 game: score 1 = A won, 0.5 = draw, 0 = A lost
Glicko.Update(a, b, out GlickoData newA, out GlickoData newB, playerAScore: 1.0);
// newA: 1563.6 / 175.4, newB: 1398.1 / 31.7

// Several games in one rating period
GlickoData updated = Glicko.Update(a, new List<(GlickoData opponent, double score)>
{
    (new GlickoData { Rating = 1400, Deviation = 30,  Volatility = 0.06 }, 1.0),
    (new GlickoData { Rating = 1550, Deviation = 100, Volatility = 0.06 }, 0.0),
    (new GlickoData { Rating = 1700, Deviation = 300, Volatility = 0.06 }, 0.0)
});
// 1464.05 / 151.52 / 0.06 - the example from Glickman's paper
```

### Win probability

```csharp
double p = Glicko.GetWinProbability(a, b); // 0.619 - uses both players' deviations
```

### Decay (inactivity)

The deviation of an inactive player grows by one Glicko-2 rating period for every elapsed `period` after a deadline
(rating and volatility don't change; deviation is capped at `DEFAULT_DEVIATION`). Reaching the deadline applies the first
period. Store the returned `nextDecayDeadline`: it keeps partially elapsed periods, so a player who logs in every day
decays gradually, not in bursts.

```csharp
// after every game: player.DecayDeadline = DateTime.UtcNow + TimeSpan.FromDays(14);

GlickoData decayed = Glicko.DecayPlayer(player.Glicko, player.DecayDeadline, DateTime.UtcNow, TimeSpan.FromDays(1),
                                        out long periodsApplied, out DateTime nextDecayDeadline);

if (periodsApplied > 0)
{
    player.Glicko        = decayed;
    player.DecayDeadline = nextDecayDeadline;
}
// e.g. deviation 60 -> 68.5 after 10 periods
```

`Local` times are converted to UTC, `Unspecified` times are treated as UTC.

---

## Proto

`DeusaldSharp.Proto`

A compact binary message format with **source-generated** serialization, for game networking and save files:

- no reflection at runtime, deterministic output,
- forward / backward compatible (fields are identified by ID, unknown fields are skipped),
- **safe on untrusted data** - malformed or malicious payloads throw `InvalidDataException`.

It is intentionally not Protocol Buffers compatible.

### Quick start

```csharp
public partial class Vec2Msg : ProtoMsgBase
{
    [ProtoField(1)] public float X;
    [ProtoField(2)] public float Y;
}

public partial class LoginMsg : ProtoMsgBase
{
    [ProtoField(1)] public int     UserId;
    [ProtoField(2)] public string? Name;
    [ProtoField(3)] public bool    IsGuest;
    [ProtoField(4)] public Vec2Msg Position = new Vec2Msg();
}

byte[] bytes = new LoginMsg { UserId = 42, Name = "Ada" }.Serialize();

LoginMsg copy = ProtoMsgBase.Deserialize<LoginMsg>(bytes); // new instance
copy.Deserialize(bytes);                                   // or into an existing instance

// Stream variants: the message takes the rest of the stream when deserializing
copy.Serialize(binaryWriter);
copy.Deserialize(binaryReader);
```

1. Derive a **`partial`** class from `ProtoMsgBase`.
2. Mark fields with `[ProtoField(id)]` - IDs are non-zero `ushort`s, unique per message.
3. The generator bundled in the `DeusaldSharp.Proto` NuGet package writes the serialization code at build time.

### Supported field types

| Kind | Types |
|---|---|
| Primitives | `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `char`, `string` |
| Built-ins | `Guid`, `DateTime`, `TimeSpan`, `Version`, `System.Net.HttpStatusCode` |
| Enums | enums marked with [`[SerializableEnum]`](#serializable-enums) |
| Nullable | `T?` for the types above; `string?`, `Version?` |
| Messages | nested messages, `ChildMsg?` |
| Collections | `List<T>` and `T[]` of all of the above (message items may be `null`), `List<T>?` |

```csharp
[SerializableEnum(SerializableEnumType.SByte)]
public enum PlayerState : sbyte { Idle = 0, Playing = 1, Disconnected = -1 }

public partial class InventoryMsg : ProtoMsgBase
{
    [ProtoField(1)] public List<int>     ItemIds     = new();
    [ProtoField(2)] public List<Vec2Msg> Checkpoints = new(); // items may be null
    [ProtoField(3)] public List<string>? Tags;                // nullable list
    [ProtoField(4)] public PlayerState   State;
    [ProtoField(5)] public PlayerState?  Previous;
    [ProtoField(6)] public Vec2Msg?      Velocity;            // nullable nested message
}
```

### Message shapes

- `public` or `internal` messages, split across several `partial` declarations.
- **Nested** message types - every containing type must be `partial` too.
- **Inheritance** - fields of base messages are serialized by derived messages (base fields must not be `private`,
  IDs must be unique across the whole chain):

  ```csharp
  public abstract partial class EntityMsg : ProtoMsgBase { [ProtoField(1)] public int EntityId; }
  public partial class PlayerMsg : EntityMsg            { [ProtoField(2)] public int Hp; }
  ```
- Auto-properties: `[field: ProtoField(n)] public int Score { get; set; }`.

### Compatibility rules

- **New reader, old data**: missing fields keep the value of a freshly constructed message (field initializers are
  respected) - also when deserializing into a reused instance.
- **Old reader, new data**: unknown fields are skipped.
- Reordering fields is safe; never reuse an ID for a different meaning.
- A `null` `string` / `Version` field is not written, so the reader keeps its default.

### Wire format

Fields are written in ascending ID order, each as a length-delimited record:

```
[ushort fieldId][int payloadLength][payload]
```

| Value | Payload |
|---|---|
| nullable value `T?` | `[bool hasValue][value]` |
| nullable message | `[bool hasValue][message fields...]` |
| message list | `[int count]` then per item `[bool hasItem]` + `[int itemLength][item]` |

Serialization needs a seekable stream (the length is written after the payload); `Serialize()` handles that for you.

### Security

`Deserialize` is safe to call on bytes from untrusted clients:

- field / item lengths and collection counts are validated against the remaining bytes **before** allocating,
- negative or oversized lengths can't cause infinite loops or huge allocations,
- nesting depth is limited by `ProtoMsgBase.MaxDepth` (default `DEFAULT_MAX_DEPTH` = 64; the root is level 1), so deeply
  nested payloads can't overflow the stack,
- every decoding failure is reported as `InvalidDataException` (with the cause as `InnerException`).

```csharp
try
{
    PlayerInputMsg input = ProtoMsgBase.Deserialize<PlayerInputMsg>(packet);
}
catch (InvalidDataException)
{
    DisconnectCheater(client);
}
```

### Build diagnostics

| ID | Problem |
|---|---|
| DSHP001 | Message is not `partial` |
| DSHP002 / DSHP003 | Invalid or duplicate field ID |
| DSHP004 | Unsupported field type |
| DSHP005 - DSHP007 | Field of a generic type parameter (or a `List` / array of it) without a `new()` constraint |
| DSHP008 | `readonly` field or property without a setter |
| DSHP009 | `static` or `const` field |
| DSHP010 | Field ID already used by a base message |
| DSHP011 | Base message field is `private` |
| DSHP012 | Containing type of a nested message is not `partial` |

---

## Upgrading to 11.0

11.0 splits the library into module packages and fixes many bugs - some fixes change behavior or the binary format.

**Packaging**
- `DeusaldSharp` is now a meta-package; modules are separate packages / dlls (same namespace, no code changes).
- The Proto source generator ships inside `DeusaldSharp.Proto`. **Remove the old `DeusaldSharp.Analyzers` package**,
  otherwise the generator runs twice.

**Binary format** - client and server must be upgraded together, and stored data written by 10.x must be re-created:
- `TimeSpan` is written as int64 ticks (was a `double` of milliseconds).
- `Version` is written with all four components (16 bytes, was 12).
- Unsigned serializable enums are no longer sign-extended.
- `ComputeHmacSha256Hex` uses UTF-8 (results differ for non-ASCII input).

**Behavior**
- `Glicko`: corrected g(φ) in the 1v1 `Update` and `GetWinProbability` (values change); new `DecayPlayer` signature.
- `Clamp` throws when `min > max`; float comparisons use a real tolerance (`MathUtils.EPSILON` = 1e-6).
- `FlagsMemory` throws for out-of-range indexes.
- Clocks tick at the exact rate, skip missed frames and have `TickException`; both are `IDisposable`.
- Exceptions in message receivers and coroutines propagate to the caller instead of breaking the bus / controller.
- Proto: malformed data throws `InvalidDataException`; messages deeper than `MaxDepth` (64) are rejected.
- `Vector2.Reflect(normal)` returns `Vector2` (recompile code built against 10.x).

---

## Building

```
dotnet build -c Release   # builds all modules, copies the netstandard2.1 dlls into UnityPackage.NetStandard2.1
dotnet test               # runs all tests
dotnet pack -c Release    # creates the NuGet packages
```

The version of every package, assembly and the Unity `package.json` is set in one place: `<Version>` in `Directory.Build.props`.

### Releasing

1. Set `<Version>` in `Directory.Build.props` and run a Release build (updates the Unity dlls and `package.json`).
2. Add the patch notes as `PatchNotes/<Version>.md` - the file becomes the body of the GitHub Release.
3. Commit, then push a tag `v<Version>` (e.g. `v11.0.0`).

The [Release workflow](https://github.com/Deusald/DeusaldSharp/blob/master/.github/workflows/release.yml) then checks that the tag matches `<Version>` and that the patch
notes exist, builds and tests the solution, publishes all packages to GitHub Packages and NuGet.org (with symbol
packages), and creates the GitHub Release with the patch notes and the `.nupkg` files attached.
NuGet.org uses Trusted Publishing (a nuget.org policy for this repository and `release.yml`, no API key secret);
GitHub Packages uses the built-in `GITHUB_TOKEN`.
