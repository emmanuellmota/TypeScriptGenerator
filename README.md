# TypeScriptBuilder

This small app to generates TypeScript type definition based on C# types.

## Supported features
- Resolving type dependency
- Generics
- Type inheritance
- Namespaces (modules)
- Enums
- Nullable types
- Dictionary converison (to strong type TS indexed objects)
- Set of code generation control attributes
- `any` for types that can't be converted

## Command line usage
    TypeScriptGenerator [input dll] [options]

### Command line options

```
    --help                              Print usage information.
    --version                           Print version number.
    -o, --output-directory <path>       Output file path (default STDOUT).
    -e, --exclude-types [names]         Classes, structs or fields/properties omitted during TS code generation.
    -c, --use-camel-case <boolean>      Changes field names form MyTestField to myTestField (default true).
    -i, --emit-i-in-interface <boolean> Adds I in interface names, MySimpleData becomes IMySimpleData (default true).
    -r, --emit-readonly <boolean>       Adds readonly to readonly fields, requires TypeScript 2.0 (default true).
    -m, --emit-comments <boolean>       Adds comments with oryginal C# type description (default false).
    -n, --ignore-namespaces <boolean>   Ignores namespace in emissions (default false).
```

Sample C# class:
```cs
class User 
{
  public int Id;
  public string Login;
  
  [TSExclude] // exclude field, can be applied on class too
  public bool Active;
}
```
Output TypeScript:
```ts
export interface IUser
{
  Id: number;
  Login: string;
}
```

## Dictionary

Dictionaries with keys of type `int` or `string` will be translated to strong typed TS indexed objects:
```cs
class Entity<T>
{
    public T Value;
}
class Test 
{
    public Dictionary<int, Entity<DateTime>> Repo;
}
```
```ts
export interface IEntity<T> {
    Value: T;
}
export interface ITest {
    Repo: { [index: number]: IEntity<Date> };
}

```
## Control attributes

### TSMap(name)
Can be used on class, struct or enum to rename generated type:
```cs
[TSMap("Funky")]
class MyCSharpClass
{
}
class Test 
{
    public MyCSharpClass Instance;
}
```
```ts
export interface Funky {
}
export interface Test {
    Instance: Funky;
}
```

### TSFlat
If applied on class (B), all fields from base classe (A) are included in class (B):
```cs
class A
{
    public int Id;
    public bool Active;
}
[TSFlat]
class B : A
{
    public string Name;
}
```
```ts
export interface B {
    Id: number;         // from A
    Active: boolean;    // from A
    Name: string;
}
```