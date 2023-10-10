# potato-eval
Expression Parser and Evaluator package for Unity/C#

| Package Name | Package Version | Unity Version |
|-----|-----|-----|
| com.potatointeractive.eval | 1.0.1 | 2019.4+ |

[Changelog](CHANGELOG.md)

# Installation

## OpenUPM
This project is available as an Open UPM Package: [![openupm](https://img.shields.io/npm/v/com.potatointeractive.eval?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.potatointeractive.eval/)

Visit [Open UPM](https://openupm.com) to learn more about the Open Unity Package Manager project and how to install the package in your Unity Project.

# Basic Usage
```csharp
using PotatoEval;
using UnityEngine;

public class MyBehaviour : MonoBehaviour {

  private Evaluator m_evaluator;
  private Context m_context;

  private void Start() {
    // Evaluator is used to parse strings to expressions, 
    // as well as evaluate expressions.
    m_evaluator = new Evaluator();

    InstructionBlock expression = m_evaluator.ToExpression("2 + 2");

    // An IContext implementation is needed to provide the
    // evaluator with variable state information such as 
    // functions, variables, and child contexts
    m_context = new Context(ContextErrorMode.Throw);

    Value value = m_evaluator.Evaluate(expression, context);

    // logs: 4
    Debug.Log(value.ToString());

  }
}
```
# Concepts

## Value Types

_Values_ are the main output given when evaluating expressions; the [`Value` struct](Assets/PotatoEval/Value.cs) provides a range of possible values and can be converted to built in C# primitive value types. You can convert a `Value` struct to these types by using an `As[Type]` property on the Value (defaulting to a Strongly typed, Checked conversion) or by using an [`IValueConverter`](Assets/PotatoEval/IValueConverter.cs) like the [`ValueConverter`](Assets/PotatoEval/ValueConverter.cs) class or your own implementation. You can also do conversions using the provided `RawNumber` and `RawString` properties of the `Value` struct if you wish to operate outside the library's rules. 

| Type | Description | Examples |
|-----|-----|-----|
| Void | An empty Value considered the default value of the `Value` struct (ie, the `RawNumber` and `RawString` are `0` and `null`) | |
| Number | Value stored as a `double` inside the `Value` struct. | `1` `3.3` `-20301` `0.233203`|
| String | A string of character stored as a `string` inside the `Value` struct. | `"string"` `"of"` `"characters"` |
| Boolean | A true or false value stored as a `double` inside the `Value` struct. | `true` `false` |
| Address | Any number of `Identifier`s strung together using the Access (`.`) operator and stored as a `string` inside the `Value` struct. | `id` `context.foo` |

## Value Of Operator

Because expressions can have _Addresses_, a special operator is used to ask your `IContext` for the value that the _Address_ represents.
```
$foo // outputs the value of foo provided by the `IContext`
```
_Addresses_ can recurse and be used to get values within child contexts of the `IContext`
```
$foo.bar // outputs the value of the `bar` value of the `foo` context of your `IContext`
```

## Assignment Operators

You can also use _Assignment_ operators to set values within your context. Note that _Assignment_ operators use an _Address_ on the left-hand side, so the Value Of Operator is not used.
```
foo = 5 // assigns the value 5 to foo within the `IContext`
```
With the regular _Assignment_ operator, you can even set the value to another Address.
```
foo = bar // assigns the value `bar` (an Address) to foo within the `IContext`
```
If you wanted to set `foo` to the value of `bar`, you would instead add a Value Of Operator to bar
```
foo = $bar // assigns the value of bar to foo within the `IContext`
```


# Language
### Constants & Literals
| Pattern | Output | Prefix |
|-----|----|----|
| `void` | `Void` | `void` keyword constant |
| `true` | `Boolean` | `true` boolean constant |
| `false` | `Boolean` | `false` boolean constant |
| `Identifier`* | `Address` | `Identifier` literal |
| `Number`* | Number | `Number` literal |
| `String`* | `String` | `String` literal |

### Prefix Operators
| Pattern | Operation | Input | Output
|----|----|----|----|
| `(`...`)` | Group | - | - |
| `!` | Logical NOT | `Boolean` | `Boolean` |
| `-` | Negate | `Number` | `Number` |
| `~` | Bitwise NOT | `Number` | `Number` |
| `$` | Value Of | `Address` | `Any` |

### Infix Operators
| Pattern | Operation | Input | Output |
|----|----|----|----|
| `(`[...`,`] `)` | Function | `Any` | `Any` | 
| `.` | Access | `Address, Address` | `Address` |
| `==` | Equals | `Any, Any` | `Boolean` |
| `!=` | Not-Equals | `Any, Any` | `Boolean` |
| `>=` | Greater Than or Equal To | `Number, Number` | `Boolean` |
| `<=` | Less Than or Equal To | `Number, Number` | `Boolean` |
| `>` | Greater Than | `Number, Number` | `Boolean` |
| `<` | Less Than | `Number, Number` | `Boolean` |
| `&&` | Logical AND | `Boolean, Boolean` | `Boolean` |
| `\|\|` | Logical OR | `Boolean, Boolean` | `Boolean` |
| `+` | Addition | `Number, Number` | `Number` |
| `-` | Subtraction | `Number, Number` | `Number` |
| `*` | Multiplication | `Number, Number` | `Number` |
| `/` | Division | `Number, Number` | `Number` |
| `%` | Modulo | `Number, Number` | `Number` |
| `<<` | Shift Left | `Number, Number` | `Number` |
| `>>` | Shift Right | `Number, Number` | `Number` |
| `&` | Bitwise AND | `Number, Number` | `Number` |
| `\|` | Bitwise OR | `Number, Number` | `Number` |
| `^` | Bitwise XOR | `Number, Number` | `Number` |
| `=` | Assignment | `Address, Any` | `Any` |
| `+=` | Addition Assignment | `Address, Number` | `Number` |
| `-=` | Subtraction Assignment | `Address, Number` | `Number` |
| `*=` | Multiplication Assignment | `Address, Number` | `Number` |
| `/=` | Division Assignment | `Address, Number` | `Number` |
| `%=` | Modulo Assignment | `Address, Number` | `Number` |
| `<<=` | Shift Left Assignment | `Address, Number` | `Number` |
| `>>=` | Shift Right Assignment | `Address, Number` | `Number` |
| `&=` | Bitwise AND Assignment | `Address, Number` | `Number` |
| `\|=` | Bitwise OR Assignment | `Address, Number` | `Number` |
| `^=` | Bitwise XOR Assignment | `Address, Number` | `Number` |
| `?`...`:`... | Ternary | `Boolean, Any, Any` | `Any` |



### Binding Power
| Category | Operators | Precedence | Associativity |
|----|----|----|----|
| Postfix | `(` `)` `.` | 150 | Left to Right |
| Unary | `-` `!` `~` `$` | 140 | Right to Left |
| Multiplicative | `*` `/` `%` | 130 | Left to Right |
| Additive | `+` `-` | 120 | Left to Right |
| Shift | `<<` `>>` | 110 | Left to Right | 
| Relational | `<` `<=` `>` `>=` | 100 | Left to Right |
| Equality | `==` `!=` | 90 | Left to Right |
| Bitwise AND | `&` | 80 | Left to Right |
| Bitwise XOR | `^` | 70 | Left to Right |
| Bitwise OR | `\|` | 60 | Left to Right |
| Logical AND | `&&` | 50 | Left to Right |
| Logical OR | `\|\|` | 40 | Left to Right |
| Conditional | `?` `:` | 30 | Right to Left |
| Assignment | `=` `+=` `-=` `*=` `/=` `%=` `<<=` `>>=` `&=` `^=` `\|=` | 20 | Right to Left |
| Comma | | 10 | Left to Right |


