# Integratieproject 1 — Academiejaar [2024-2025]

## Teamnaam
Whimps/24

## Teamleden Development
- Jurgen Potter
- Xander Verbruggen
- Lars Veringmeier
## Teamleden Deployment
- Jarne Mariën
- Senne Hellemans

## Korte Omschrijving
Dit project is ontwikkeld in het kader van het vak **Integratieproject 1**.  
Het doel van het project is om een platform te maken voor organisaties om de meningen van de bevolking op te kunnen volgen.

## Start instructies
Run eerst vanuit de ClientApp de volgende commandos
```
npm install
npm run build
```

## Testgebruikers

| Gebruikersnaam         | Wachtwoord | Rol          | Extra info                                   |
|------------------------|------------|--------------|----------------------------------------------|
| `admin@example.com`    | `Test1!`   | Admin        | IsSuper: true                                |
| `admin2@example.com`   | `Test1!`   | Admin        |                                              |
| `antwerpen@example.com`| `Test1!`   | Organization | TenantId: `antwerpen`, IsSuper: true         |
| `brussel@example.com`  | `Test1!`   | Organization | TenantId: `brussel`                          |
| `paul@example.com`     | `Test1!`   | Member       | Naam: Paul Veenstra, TenantId: `antwerpen`, Geboortejaar: 1980 |

- antwerpen@example.com heeft 2 panels 1ste panel is al gestart
- het 2de panel heeft al aanmeldingen.
- het 2de panel van antwerpen heeft geseede uitnodigingen die gebruikt kunnen worden
- paul@example.com is lid van panel 1 van antwerpen

- Invitatie HEMf-Xu0L-1ETh-urZJ-26s9 kan zich aanmelden voor panel 2
- Invitatie blLy-Tvxl-1TXG-nYg3-CBtD moet zich nog registreren en hierna geloot (getrokken) worden