# and why he 😂
hi! this is a celeste mod that adds various variant-like options and other features to the game. these things include
- physics options
- visual options
- miscellaneous options
- some entities
    - fluid simulation thing
- some triggers
    - triggers for the aforementioned options
    - some other stuff
- weird other things

# contributing (why would you ever contribute to this project 😭)
feel free to make a pr with something
- i really need experience working with other people on these things honestly
- hopefully my code isnt too unreadable

## style guidelines
the general rule for this one to just try to be consistent with the rest of the codebase. i would recommend looking around a bit to see what i mean by this.

the really important things are
- newline braces are evil
- spaces should not go before the parentheses on if/for/while/switch/etc statements
- anything thats public or protected is PascalCase
- anything thats private is camelCase

the less important things (but still important) are
- use empty lines to separate blocks of code into distinct "concepts"
    - this one is very subjective, but almost any file in this codebase should have a good example
- ternary expressions and expression bodied functions put their operators at the very start of a new line in most cases
- please use the var keyword instead of explicitly declaring the type
    - if you need to explicitly declare the type, please use ``default(TypeName)``

## gooberhelper-specific things
you should try to avoid manually hooking things. you can add the ``[OnHook]`` or ``[ILHook]`` attribute to any method to automatically hook it. examples of this can be found throughout the entire codebase

when adding an option, the bare minimum list of files you need to modify is
- Options/OptionsManager.Categories.cs
- Options/OptionsManager.Options.cs
- Dialog/*.txt
- Loenn/triggers/gooberHelperOptions.cs

have fun (?) contributing!!!

#
![buh](https://media1.tenor.com/m/ITa81jyIccYAAAAd/rinmya-gleep-glorp.gifmya-gleep-glorp.gif "hi")