# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.4.0] - 2026-03-12
### Added
- Added `Cleanup()` method to `IFlexTargetRepository` interface
- Added `Cleanup` implementation to `BuiltinFlexTargetRepositoryAccessor` and `BuiltinFlexTargetRepository`
- Added new feature: Flex Target Adapter
    - The new `FlexTargetAdapter<T>` class wraps any type into an object that implements `IFlexTarget`, allowing it to be used in the core targeting methods
    - In order for the adapter to work, 3 static functions must be defined in order to fully implement the `IFlexTarget` interface. These functions can be defined for the type that is being adapted using the method: `FlexTargetAdapter.SetAdapterFuncs<T>`, or with the more convenient `FlexTargetAdapter.Adapt<T>` extension method
    - This new feature is primarily meant for one-off scenarios where performance and memory allocation is not a primary concern, such as debug commands. Use the `FlexTargetAdapter.Adapt<T>` method to adapt a list of objects that you want to adapt. This method will create new adapter objects and return a new list of adapters to use in the core methods. (This, of course, allocates memory. However, the core methods will not allocate memory as usual)
    - Added another unit test file: `AdapterTest.cs`
- Added `Documentation~` folder and image that will be used in documentation
### Changed
- Updated `FlexTargetRepository.ReplaceRepository` signature to optionally call `Cleanup` on the internal repository

## [0.3.0] - 2026-02-04
This version has some backward **incompatible** changes.
### Removed
- Removed virtual `OnDrawGizmosSelected()` from `FlexTargetComponent.cs` since it tends to not be useful at all in real world practice.
### Changed
- Changes to `FlexTargetComponent.cs`
    - `IsTargetValid` no longer has a default implementation. It is now the responsibility of the child class to implement.
    - Changed the default implementation of `RegisterTargetWhen` to register OnEnable. I think it makes more sense for most situations, since registering OnAwake might add more targets to the repository that constantly need to be filtered out if they are not enabled.
- Changed builtin targeting data fields to use tooltips from `FlexTargetingTooltips`
### Added
- Added static class: `FlexTargetingExtras.Debug` , and static getter: `LastScoredTargetList`. This gives advanced access to the internal list of potential targets and their associated score values that was just populated and sorted by the last called core method. Useful for debugging what score values are being assigned to targets.
- Added support for the line-of-sight check either using a Raycast or Spherecast. `IFlexData` now has a `LosRaySize` getter. It has a default implementation returning 0, so this change will not break backward compatibility. A value of 0 makes the LOS check use a Raycast. Otherwise, this value will define the sphere radius in a Spherecast.
    - Also added `_losRaySize` serialized field and getter/setter to all builtin targeting data
- Added `FlexTargetingTooltips` static class with const strings containing common tooltips for flex data and flex target fields.

## [0.2.0] - 2026-01-31
### Changed
- Changes to `FlexTargetComponent.cs`
    - The "RegisterWhen" field is no longer serialized and editor facing. Instead it is now a virtual property that can be overriden in child classes. This makes more sense since it isn't typical for that option to potentially be different from target to target. Child classes can choose to handle this as they wish.
    - Added some more documentation
- Updated all source files to contain headers with copyright information
- Updated package info with a new description, changed author URL, and documentation URL

## [0.1.0] - 2026-01-30
### This is the first pre-release of Flex Targeting
The package is currently undergoing real-world testing. Full documentation, detailed changelogs, and a proper README is coming soon.