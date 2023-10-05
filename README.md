# potato-eval
Expression Parser and Evaluator package for Unity/C#

| Package Name | Package Version | Unity Version |
|-----|-----|-----|
| com.potatointeractive.eval | 1.0.0 | 2019.4+ |

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

# Expression Syntax

## Value Types

After parsing 

| Type | Description | Examples |
|-----|-----|-----|
| Number | Value stored as a `double` inside the `Value` struct. Can be converted to primitive number types recognized by C# (all except `decimal`) provided that the falls within the valid range of the primitive type. | `1` `3.3` `-20301` `0.233203`|
| String | A string of character stored as a `string` inside the `Value` struct. Can only be converted to a C# `string`. | `"string"` `"of"` `"characters"` |
| Boolean | A true or false value stored as a `bool` inside the `Value` struct. Can only be converted to a C# `bool`. | `true` `false` |
| Address | Any number of `Identifier`s strung together using the Access (`.`) operator and stored as a `string` inside the `Value` struct. | `id` `context.foo` |



## Value Of Operator