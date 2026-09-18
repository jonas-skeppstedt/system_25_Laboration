# 🚦 Laboration System25 – GitHub-flöde och CI

Samma kod och samma tester som i TDD-laborationen. Det som tillkommer är processen
runtomkring: ingen kan förstöra `main`, varje ändring granskas av en människa, och
maskinen kör era tester varje gång.

## 🎯 Lärandemål

- Skriva en GitHub Actions-workflow från en tom fil
- Skilja på **kontexter** och **miljövariabler**, och veta var respektive går att använda
- Förstå skillnaden mellan en pipeline som *rapporterar* och en som *stoppar*
- Skydda `main` så att trasig kod inte kan mergas
- Arbeta i flödet issue → gren → pull request → granskning → merge

## 🛠️ Innan ni börjar

**Utgångsläge.** Ni arbetar i ert eget repo från TDD-laborationen. Ställ er på `main`
och kör `dotnet test` lokalt *innan* ni gör något annat. Är något rött: fixa eller ta
bort det först. Annars blir er första CI-körning röd av skäl som inte har med
workflowen att göra, och ni kommer att leta på helt fel ställe.

**Ert repo måste vara publikt.** Branch protection finns inte på privata repon utan
betald plan. Settings → General → längst ner → Change visibility.

**Lägg till din partner:** Settings → Collaborators. Utan det kan hen inte godkänna
era pull requests.

**Jobbar ni ensamma?** Sätt *inte* godkännandekravet till noll — då försvinner både
den blockerade mergen i Del 1 och granskningen i Del 3. Lägg istället till en av oss
som collaborator, så granskar och godkänner vi era pull requests. Del 3 gör ni då som
två PR:er efter varandra istället för en var.

---

# 🟢 Del 1 – En CI som stoppar dig

### 1. Skriv `.github/workflows/ci.yml`

Ni skriver den här filen själva. Mappen måste heta exakt `.github/workflows/`, punkten
inkluderad, i roten av repot.

> 🚫 GitHub erbjuder en färdig .NET-mall under fliken Actions. **Använd den inte.**
> Att få strukturen i fingrarna är halva laborationen — och den mallen gör saker ni
> inte har bett om.

**Beställning:**

| Ska finnas | Värde |
|---|---|
| Ett namn på workflowen | `CI` |
| Tre triggers | push till `main`, pull request mot `main`, och manuell körning |
| Ett jobb | id `build-and-test`, visningsnamn **Bygg och testa** |
| Körs på | `ubuntu-latest` |

Jobbet ska, i den ordningen:

1. Hämta hem koden — varje körning startar i en tom maskin
2. Installera .NET `10.0.x`
3. `dotnet restore`
4. `dotnet build --no-restore --configuration Release`
5. `dotnet test --no-build --configuration Release`

De två färdiga byggstenarna ni behöver heter `actions/checkout@v7` och
`actions/setup-dotnet@v6`. Den senare tar ett värde — leta upp vad det heter.

Orden ni kommer att behöva: `name`, `on`, `jobs`, `runs-on`, `steps`, `uses`, `with`,
`run`. Alla finns förklarade i syntaxdokumentationen.

📖 [Workflow-syntax](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax)
· [setup-dotnet](https://github.com/actions/setup-dotnet)
· [Trigger-typer](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows)

Frågor att fundera på medan ni skriver: varför gör vi `restore`, `build` och `test` som
tre separata steg istället för ett? Vad händer med teststeget om bygget failar?

> 💡 **YAML tillåter inga tabbar**, bara mellanslag, och indenteringen bär betydelse.
> Blir det fel syntax säger GitHub till i Actions-fliken.

Pusha till `main` och titta på fliken **Actions**.

> ⚠️ **Låt den köra klart innan ni går vidare.** GitHub kan bara kräva en kontroll som
> den har sett köra tidigare.

### 2. Badge i README

```markdown
![CI](https://github.com/<användarnamn>/<repo>/actions/workflows/ci.yml/badge.svg)
```

### 3. Skydda `main`

**Settings → Rulesets → New ruleset → New branch ruleset**

- Enforcement: `Active`, target: `Include default branch`
- ✅ Require a pull request before merging → Required approvals: `1`
- ✅ Require status checks to pass → lägg till **Bygg och testa**

### 4. Gör sönder något med flit

Hoppa inte över det här — det är hela poängen med Del 1.

```bash
git switch -c trasig-kod
```

Ändra i **produktionskoden** så att ett test faller. `>` → `>=` i `CountIncreases` är
ett förslag, men kör `dotnet test` lokalt och **kontrollera att det verkligen blir
rött** innan ni pushar. Blir det fortfarande grönt har ni hittat ett hål i era egna
tester — det är också ett resultat. Välj då något annat att ha sönder.

Pusha grenen och öppna en pull request mot `main`.

Läs nu noga vad GitHub skriver längst ner i pull requesten. Mergen är blockerad av
**mer än en sak**:

- kontrollen **Bygg och testa** är röd
- och ingen har godkänt ändringen

Skriv ner båda. Åtgärda dem **en i taget** och se hur meddelandet ändras: först
testerna gröna, sedan partnerns **Approve**. Merge-knappen vaknar först när samtliga
villkor är uppfyllda — ett grönt kryss räcker inte.

Ni äger repot och har alla rättigheter. Ni kan ändå inte merga. Det är meningen.

---

# 🟡 Del 2 – Kontexter och variabler

Er pipeline fungerar, men den berättar ingenting om sig själv och den upprepar ordet
`Release` på två ställen. Det ska den sluta med.

Det här är **två ändringar**, och i Del 3 tar ni var sin av dem och skickar in den som
en pull request som den andra granskar. Bestäm redan nu vem som tar vad:

- **2.1–2.3** — variabler, kontext och sammanfattning → den enas PR
- **2.4** — formateringsjobbet → den andras PR

Ingen färdig YAML här heller. Beställning, skelett och TODO — precis som `Sonar.cs`
gjorde mot er i förra laborationen. Det finns inget facit; ropa på oss istället.

> 🔀 **Ni kan inte pusha till `main` längre.** Från och med Del 1 går varje ändring via
> en gren och en pull request:
>
> ```bash
> git switch main && git pull
> git switch -c min-andring
> # ändra, commita, pusha
> git push -u origin min-andring
> ```
>
> Sedan öppnar ni en PR, den andra godkänner, och ni mergar. Del 3 gör det här på
> riktigt med issue och granskning — här räcker det att ni vet hur ändringen tar sig
> in.

### 2.1 En variabel istället för två `Release`

`--configuration Release` står på två ställen i er fil. Ska ni någon gång byta till
`Debug` ska det räcka att ändra på ett.

**Beställning:** definiera `BUILD_CONFIGURATION` **en gång** och använd den i både
bygg- och teststeget.

```yaml
# TODO: lägg till en env-nyckel med BUILD_CONFIGURATION: Release.
# env kan stå på tre nivåer — hela workflowen, ett jobb, eller ett enskilt steg.
# Vilken nivå är rätt här, och varför?

      - run: dotnet build --no-restore --configuration ???
      - run: dotnet test --no-build --configuration ???
```

Det finns två sätt att läsa variabeln i ett `run`-steg: `$BUILD_CONFIGURATION` och
`${{ env.BUILD_CONFIGURATION }}`. Båda fungerar här. Ta reda på vad skillnaden är — det
är samma skillnad som 2.2 handlar om.

### 2.2 Vem startade körningen, och varför?

`github`-**kontexten** vet allt om körningen: vem som utlöste den, vilken händelse det
var, vilken gren det gäller.

**Beställning:** ett steg som hämtar användaren och händelsen ur `github`-kontexten,
skickar in dem som miljövariabler till steget, och skriver ut dem — både i loggen och i
sammanfattningen.

```yaml
      - name: Vem och vad startade det här?
        env:
          # TODO: hämta värdena ur github-kontexten. Syntaxen är ${{ ... }}.
          # Ni behöver tre saker: den som startade körningen, namnet på
          # händelsen, och det korta namnet på grenen.
          STARTAD_AV: ${{ ... }}
          HANDELSE: ${{ ... }}
          GREN: ${{ ... }}
        run: |
          echo "Startad av $STARTAD_AV — händelse: $HANDELSE — gren: $GREN"
          # TODO: skriv samma rad till sammanfattningen också.
          # Allt som skrivs till filen $GITHUB_STEP_SUMMARY dyker upp som
          # markdown överst på körningens sida.
```

Här sitter poängen med hela Del 2. `${{ github.actor }}` och `$GITHUB_ACTOR` ger samma
värde, men det är två olika maskinerier:

- `${{ ... }}` utvärderas av **Actions**. Skalet får aldrig se uttrycket, bara
  resultatet.
- `$GITHUB_ACTOR` expanderas av **skalet**, när kommandot körs.

Därför fungerar `${{ ... }}` på rader där det inte finns något skal inblandat — `if:`,
`name:`, `runs-on:` — medan `$VARIABEL` bara betyder något inuti ett skalkommando.

Vilka kontexter som går att använda beror dessutom på *var i filen* ni står. Vissa
finns överallt, andra först när jobbet börjat köra. Det är därför ni inte kan läsa ett
tidigare stegs utdata var som helst.

📖 [Kontexter](https://docs.github.com/en/actions/concepts/workflows-and-actions/contexts)
· [Var en kontext går att använda](https://docs.github.com/en/actions/reference/workflows-and-actions/contexts#context-availability)
· [Contexts reference](https://docs.github.com/en/actions/reference/workflows-and-actions/contexts)
· [Variables reference](https://docs.github.com/en/actions/reference/workflows-and-actions/variables)

### 2.3 Förutsäg först, kontrollera sedan

Fyll i tabellen **innan** ni kör något. Gissa.

| | `github.event_name` | `github.ref_name` | `github.actor` |
|---|---|---|---|
| Push till `main` | | | |
| Pull request mot `main` | | | |
| Manuell körning | | | |

Kör dem sedan i **den här ordningen** — `main` är skyddad, så ni kommer åt dem i just
den följden:

1. **Pull request.** Öppna PR:en med er ändring. Den körningen är `pull_request`.
2. **Push.** Merga PR:en. Själva mergen är en push till `main` och ger en ny körning.
3. **Manuell.** Nu ligger den uppdaterade workflowen på `main`. Gå till Actions →
   **Run workflow**.

Jämför med era gissningar. Minst en ruta kommer att överraska er — titta särskilt på
`ref_name` i PR-körningen. Vilken, och varför?

### 2.4 Formatering som eget jobb

Två jobb istället för ett gör att ni kan skilja "koden fungerar inte" från "koden är
slarvigt formaterad" — och de körs samtidigt.

**Beställning:** ett jobb som failar när koden inte är formaterad, utan att ändra
någonting.

```yaml
  formatering:
    name: Kodformatering
    runs-on: ubuntu-latest
    steps:
      # TODO: hämta koden och installera .NET, precis som i jobbet ovanför
      # TODO: kör dotnet format med den flagga som rapporterar istället för
      #       att rätta. Kör "dotnet format --help" och leta.
```

Failar det? Kör `dotnet format` lokalt utan flaggor — då rättar den allt åt er, och
`git diff` visar vad det var.

Lägg märke till vad ett `run`-steg egentligen är: **ett kommando, och dess felkod
avgör resten.** `dotnet format --verify-no-changes` avslutar med noll när allt är
snyggt och med något annat när det inte är det — och det är precis det som gör steget
rött. Ett jobb är en kedja av sådana steg. Allt ni kan köra i en terminal kan bli en
kontroll.

> Glöm inte att lägga till **Kodformatering** som obligatorisk i rulesetet — annars
> stoppar den ingenting.

---

# 🔴 Del 3 – Arbeta som ett team

Ni har två ändringar från Del 2. Nu ska de in i repot på riktigt — en var, granskad av
den andra. Ping-pong-programmering, fast på PR-nivå.

1. **Ett issue var.** Beskriv er ändring i en mening: vad och varför. Notera numret.
2. **Var sin gren.** `git switch main && git pull && git switch -c ...`
3. **Var sin pull request.** Skriv `Fixes #12` i beskrivningen — då stängs issuet
   automatiskt när PR:en mergas.
4. **Granska varandras.** Den som *inte* skrev filen går till **Files changed** och
   lämnar minst en riktig kommentar. Inte "ser bra ut" — utan: körde den grönt? Ligger
   `env` på rätt nivå? Skulle jag förstå den här raden om jag inte hade skrivit den?
   Vad händer om händelsen är något annat än en push? Sedan **Approve**.
5. **Squash and merge.** Kolla att issuet stängdes av sig självt.

När båda är mergade har var och en av er skrivit ett stycke workflow, granskat ett
annat, och sett CI:n köra på sin egen pull request.

---

# 🎓 Individuell kontroll

15–20 minuter, **var för sig**, från en **tom fil**. Ni gör den samtidigt, var och en
vid sin dator.

> Skapa en workflow som kan startas manuellt. Den ska ha ett jobb som körs på Ubuntu,
> hämta koden, installera .NET och köra testerna. Använd en egen miljövariabel för
> byggkonfigurationen, och skriv ut vem som startade körningen.

**Ni ska inte köra den.** Skriv filen i en tom mapp på er egen dator — spara den som
`kontroll.yml`, committa ingenting, pusha ingenting. Det är filen vi bedömer.

Anledningen är värd att känna till: en manuellt startad workflow får sin **Run
workflow**-knapp först när filen ligger på repots default-gren. I ert skyddade repo
kräver det en pull request och partnerns godkännande — mitt i en övning där partnern
inte får hjälpa till. Vill ni se den köra gör ni det efteråt, i ett eget testrepo.

**Regler:** dokumentationen är tillåten. Partnern är det inte, och inte heller
AI-assistenter eller en färdig workflow att utgå från — varken er egen från förmiddagen
eller någon annans.

När tiden är ute kommer vi till er med filen uppe på er skärm. Ni ska kunna:

- förklara **ett val** ni gjort — varför den nivån, varför den triggern, varför just så
- göra **en liten ändring** medan vi tittar

Det här är inte ett prov på att memorera YAML. Det är en kontroll på att båda i paret
kan bygga en pipeline, inte bara den som råkade hålla i tangentbordet.

---

## 🗣️ Redovisning

Ingen inlämning. Ni visar upp det på plats, med repot uppe på skärmen:

- Den pull request där mergen blockerades — och de **två** skäl GitHub gav
- Era två mergade PR:er från Del 3, var och en godkänd av den andra
- Ett issue som stängdes av sig självt
- En körning i Actions-fliken med er sammanfattning
- Er ifyllda tabell från 2.3, och vilken ruta som överraskade er

---

# 🔶 Valfritt

### 🛡️ En egen kontroll: ingen pusselindata i repot

Advent of Code ber uttryckligen att man **inte** publicerar sin pusselindata. Ert repo
är publikt. Det här är alltså ett riktigt problem, inte ett påhittat.

**Beställning:** ett jobb som failar om någon spårad `input.txt` innehåller data. Det
behöver inte .NET alls och är klart på fem sekunder.

```yaml
  indata:
    name: Ingen pusselindata incheckad
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v7
      - shell: bash
        run: |
          # TODO: gå igenom varje input.txt som git spårar.
          # Är någon av dem icke-tom: skriv ut vilken, och avsluta med exit 1.
          #
          # Tips: git ls-files -- '*input.txt' listar dem.
          #       [ -s "$fil" ] är sant om filen finns OCH inte är tom.
          #       echo "::error::..." gör meddelandet rött i gränssnittet.
```

Testa den med **påhittad** data — skriv `123` i en `input.txt`, aldrig er riktiga
indata. Pusha grenen **och öppna en pull request**, annars händer ingenting: er
workflow lyssnar på push till `main` och på pull requests mot `main`, inte på pushar
till vilken gren som helst.

När den har kört en gång: lägg till **Ingen pusselindata incheckad** som obligatorisk
kontroll i rulesetet. En kontroll som inte står där stoppar ingenting — den blir bara
ett rött kryss som går att merga förbi.

Lägg också märke till gränsen för vad den kan göra. Den hindrar att indatan *mergas*
till `main`. Den kan inte ogöra att ni pushade den till en publik gren. Det som en gång
nått GitHub finns kvar.

### 🚢 En nedladdningsbar release

Svaret på "hur ger jag mitt program till någon som inte har .NET?"

```yaml
name: Release
on:
  push:
    tags: ['v*']
permissions:
  contents: write
jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v7
      - uses: actions/setup-dotnet@v6
        with:
          dotnet-version: '10.0.x'
      - run: >
          dotnet publish Y2021D01/Y2021D01.csproj -c Release
          --runtime linux-x64 --self-contained true
          -p:PublishSingleFile=true --output publicerat
      # Programmet läser input.txt bredvid sig själv. Skickar ni bara
      # binären kraschar den hos mottagaren — paketera hela mappen.
      - run: tar -czf y2021d01-linux-x64.tar.gz -C publicerat .
      - run: gh release create "$GITHUB_REF_NAME" y2021d01-linux-x64.tar.gz --generate-notes
        env:
          GH_TOKEN: ${{ github.token }}
```

`git tag v1.0.0 && git push origin v1.0.0` — och titta under **Releases**.

Ladda ner arkivet, packa upp det i en tom mapp och kör det. Skriv er indata i
`input.txt` bredvid binären, så räknar den på den. Bygget gäller **Linux x64** —
`--runtime` avgör det, och en Windows-mottagare behöver `win-x64` istället.

> Testa alltid er egen release genom att packa upp den i en tom mapp och köra den där.
> Det är först då ni ser om ni glömt något som bara fanns på er egen dator.

### Övrigt

- **Testresultat i sammanfattningen** — spara utdatan från `dotnet test` med `tee`,
  plocka ut raderna med `grep`, skriv dem till `$GITHUB_STEP_SUMMARY`. Två fällor:
  steget måste ha `shell: bash`, annars tappas felkoden i pipen och **jobbet blir grönt
  trots röda tester** — kontrollera det genom att låta ett test faila med flit och se
  att CI:n blir röd. Och ett steg hoppas normalt över när ett tidigare failat, vilket
  är precis när ni vill se resultatet: leta upp *status check functions*
- **Dependabot** (`.github/dependabot.yml`) — en robot som öppnar PR:er när paket blir
  gamla, och er CI granskar robotens förslag
- **gitleaks** som eget jobb — letar hemligheter i historiken
- **Testtäckning** — `dotnet test --collect:"XPlat Code Coverage"`
- **`actions/cache`** på NuGet-paketen — mät tiden före och efter

---

## 🧯 När det inte funkar

| Symptom | Orsak |
|---|---|
| "Bygg och testa" syns inte i rulesetet | Kontrollen måste ha kört en gång först |
| Merge-knappen grå fast du äger repot | Rätt så. Rulesets gäller även ägare |
| Merge-knappen grå fast testerna är gröna | Godkännandet saknas — det är två villkor |
| Kan inte godkänna din egen PR | Partnern måste vara collaborator |
| Workflowen startar inte alls | Ligger filen i `.github/workflows/*.yml`? Tabbar i YAML? |
| Inget händer när jag pushar min gren | Ni lyssnar på `main` och på PR:er — öppna en PR |
| Grönt lokalt, rött i CI | Beroende av en gitignorerad fil? Molnet har bara det som är incheckat |

## 📚 Resurser

- [GitHub Actions](https://docs.github.com/en/actions) ·
  [Workflow-syntax](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax) ·
  [Kontexter](https://docs.github.com/en/actions/concepts/workflows-and-actions/contexts) ·
  [Rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets) ·
  [Länka PR till issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/using-issues/linking-a-pull-request-to-an-issue)

Fastnar ni: fråga direkt. Att stirra på YAML-indentering är ingen lärdom, och vi
sitter här för att ni ska komma vidare.

**Lycka till! 🚀**
