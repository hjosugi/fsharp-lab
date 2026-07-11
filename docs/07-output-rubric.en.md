<!-- i18n: language-switcher -->
[English](07-output-rubric.en.md) | [日本語](07-output-rubric.md)

# Output Rubric

Self-assess each Module on a scale of 0 to 3.

| Score | Meaning |
|---:|---|
| 0 | Not read or executed |
| 1 | Can implement while referencing the material |
| 2 | Can implement and explain without documentation |
| 3 | Can adapt to a different domain and explain trade-offs |

## Must-Check

- [ ] Can explain input, output, and failure with only the type signature
- [ ] Can enumerate non-happy paths using DU
- [ ] Can test the pure core without mocks
- [ ] Can replace I/O dependencies with functions/records
- [ ] Can explain why code that creates invalid states does not compile
- [ ] Can suggest an alternative design
- [ ] Can port to the Parking domain

Passing requires all scores to be at least 2, and Modules 4 through 10 must be scored 3.