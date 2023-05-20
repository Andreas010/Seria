# Seria
An API for serializing any class in C#, provided you have a handler for it's field types!

!!! Currently still very much in development !!!

Features:
- Provides out-of-the-box support for generic C# types
- Simple Base class for extending
- Serialisation done completely in memory
- Attribute for deciding what fields to save

Roadmap:
- [ ] Finish implementing the generic type handlers
- [ ] Add custom exceptions
- - [ ] Missing Type Handler
- - [ ] Missing SeriaValue Field
- [ ] Add Serialisation Flags
- - [ ] Weak Mode
- - [ ] Normal Mode
- - [ ] Strict Mode
- [ ] Support for properties
- [ ] Allow custom SeriaValue name if variable names change
- [ ] Attribute for serializing an entire class/struct
