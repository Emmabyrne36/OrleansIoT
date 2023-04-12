# Orleans IoT

Simple application demonstrating various features within Microsoft Orleans including:

- Grain state and persistence
  - Using a custom persistence provider
- Grain optimisation
  - Stateless worker grains
  - Re-entrant grains
  - Immutable messaging
- Timers and Observers
- Sending messages to Orleans grains via a WebAPI

The application consists of several grains that interact with each other via message passing and simulate recording the temperature of an individual device as well as the entire distributed system.
