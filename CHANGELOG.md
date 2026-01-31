# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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