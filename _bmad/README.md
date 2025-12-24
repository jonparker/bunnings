# BMAD V6 Installation

This project has been set up with **BMAD V6** (Build More, Architect Dreams) - an AI-driven agile development framework.

## Installation Details

- **Version**: 6.0.0-alpha.20
- **Installation Date**: 2025-12-24
- **Framework**: BMad Core v6

## Directory Structure

```
_bmad/
├── _config/           # Configuration files
├── core/             # BMad Core framework
├── modules/          # Installed modules
│   ├── bmm/         # BMad Method - Agile Development
│   ├── bmb/         # BMad Builder - Custom Agent Creator
│   ├── cis/         # Creative Innovation Suite
│   └── bmgd/        # BMad Game Development
└── utility/          # Utility files

_bmad-output/
├── planning-artifacts/        # Analysis, Planning, Solutioning outputs
└── implementation-artifacts/  # Implementation phase outputs

docs/                 # Long-term project documentation
```

## Installed Modules

### BMad Method (BMM)
Complete agile development framework with 12 specialized AI agents and 34+ workflows across 4 phases:
- **Analysis** - Brainstorm, research, and explore solutions
- **Planning** - Create PRDs, tech specs, and design documents
- **Solutioning** - Design architecture, UX, and technical approach
- **Implementation** - Story-driven development with continuous validation

### BMad Builder (BMB)
Create custom AI agents, workflows, and modules tailored to your specific needs.

### Creative Innovation Suite (CIS)
Tools and agents for creative problem-solving and innovation.

## Getting Started

### 1. Initialize Your Project

Load any BMAD agent in your IDE and run:

```
*workflow-init
```

This command analyzes your project and recommends the appropriate workflow track.

### 2. Choose Your Workflow Track

BMAD adapts to your needs with three intelligent tracks:

| Track | Use For | Planning | Time to Start |
|-------|---------|----------|---------------|
| **Quick Flow** | Bug fixes, small features | Tech spec only | < 5 minutes |
| **BMad Method** | Products, platforms | PRD + Architecture + UX | < 15 minutes |
| **Enterprise** | Compliance, scale | Full governance suite | < 30 minutes |

### 3. Access Available Agents

Agents are located in `_bmad/modules/*/agents/`. Common agents include:

- **Developer** - Implementation and coding tasks
- **Architect** - System design and architecture
- **PM** (Product Manager) - Requirements and planning
- **UX Designer** - User experience and interface design
- **Test Architect** - Testing strategy and implementation

### 4. Run Workflows

Workflows are located in `_bmad/modules/*/workflows/`. Execute workflows using:

```
*workflow-[workflow-name]
```

## Configuration

The main configuration file is located at `_bmad/_config/config.yaml`. This file contains:

- Project settings (name, type, root path)
- Module configurations
- Output folder locations
- User preferences (skill level, language)

## IDE Integration

BMAD V6 supports 18+ IDEs including:
- Claude Code
- Cursor
- Windsurf
- VS Code
- And more...

## Documentation

- [BMAD Official Documentation](https://github.com/bmad-code-org/BMAD-METHOD)
- [BMad Method Module Docs](_bmad/modules/bmm/docs/index.md)
- [BMad Builder Docs](_bmad/modules/bmb/docs/index.md)

## Community

- **GitHub**: https://github.com/bmad-code-org/BMAD-METHOD
- **Discord**: https://discord.gg/gk8jAdXWmj
- **YouTube**: https://www.youtube.com/@BMadCode

## Version Information

- **BMAD Core**: v6.0.0-alpha.20
- **Installation Method**: Manual setup via repository clone
- **License**: MIT

## Notes

- Planning artifacts (Phases 1-3) output to `_bmad-output/planning-artifacts/`
- Implementation artifacts (Phase 4) output to `_bmad-output/implementation-artifacts/`
- Long-term project knowledge outputs to `docs/`
- The `_bmad-output/` directory is ignored by git (temporary artifacts)
- The `_bmad/` directory is tracked in git (framework files)
