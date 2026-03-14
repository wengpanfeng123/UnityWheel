---
name: superpowers-mode-switch
description: Use when user wants to enable or disable the full Superpowers development workflow
---

# Superpowers Mode Switch

## Overview

This skill provides a simple on/off switch to control whether the full Superpowers development workflow is activated. When enabled, all Superpowers skills will be automatically invoked according to their triggering conditions. When disabled, the agent works in a more direct, ad-hoc manner.

## Trigger Commands

| Command | Action |
|---------|--------|
| `/superpowers on` | Enable Superpowers workflow |
| `/superpowers off` | Disable Superpowers workflow |
| `/superpowers status` | Show current status |

## Behavior

### When Enabled (Superpowers ON)

The agent will:

1. **Always use brainstorming** before any creative work
2. **Always use writing-plans** before implementation
3. **Always use git worktrees** for isolated development
4. **Always use TDD** for implementation
5. **Always use verification** before completion
6. **Always use proper code review** workflows

The agent will announce: "🔛 Superpowers mode ENABLED - using full development workflow"

### When Disabled (Superpowers OFF)

The agent will work more directly:

1. Ask clarifying questions as needed
2. Write and modify code directly
3. Skip formal planning phases
4. Use simpler, faster workflows

The agent will announce: "🔕 Superpowers mode DISABLED - using direct workflow"

## Quick Reference

| Mode | Trigger | Workflow |
|------|---------|----------|
| ON | `/superpowers on` | Full Superpowers workflow |
| OFF | `/superpowers off` | Direct ad-hoc workflow |
| Status | `/superpowers status` | Show current mode |

## How It Works

This skill sets a session state that affects all other Superpowers skills:

- **ON**: Skills auto-trigger based on their conditions
- **OFF**: Skills only trigger when explicitly invoked via Skill tool

## Default State

The default state is **ON** (Superpowers enabled) unless the user explicitly turns it off.

**User instructions always take precedence** - if the user gives explicit directions, those override the mode setting.
