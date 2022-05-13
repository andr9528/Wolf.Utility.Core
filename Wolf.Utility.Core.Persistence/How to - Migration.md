# Migration

source: <https://www.entityframeworktutorial.net/efcore/entity-framework-core-migration.aspx>

Commands:

- Add
    - Packet Manager Console
        - Set default project to match project containing 'Context' class, then use command.
        - Command: add-migration <migration name>
    - Dotnet Console Line Interface
        - Command: Add <migration name>
    - Description: Creates a migration by adding a migration snapshot.
- Remove
    - Packet Manager Console
        - Command: Remove-migration
    - Dotnet Console Line Interface
        - Command: Remove
    - Description: Removes the last migration snapshot.
- Update
    - Packet Manager Console
        - Command: Update-database
    - Dotnet Console Line Interface
        - Command: Update
    - Description: Updates the database schema based on the last migration snapshot.