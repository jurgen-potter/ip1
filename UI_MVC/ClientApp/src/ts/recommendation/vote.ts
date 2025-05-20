document.addEventListener('DOMContentLoaded', () => {
    // Zoek alle formulieren met de klasse 'vote-form'
    const voteForms = document.querySelectorAll<HTMLFormElement>('#vote-form');
    // Houd bij op welke items al gestemd is 
    const votes: Record<string, boolean> = {};
    const tenant = window.location.pathname.split('/')[1];

    // Functie om één knop bij te werken op basis van stemstatus
    const updateButton = (form: HTMLFormElement, voted: boolean) => {
        // Vind de knop in het formulier
        const btns = form.getElementsByTagName('button');
        if (!btns) return; // typescript validatie
        // Past de tekst van de btn aan
        if (!voted) {
            btns[0].classList.add('btn-success');
            btns[0].classList.remove('btn-primary');
            btns[0].textContent = 'Stem voor'
            btns[1].hidden = false;
            btns[1].textContent = 'Stem tegen'
            // Als je deze tekst aanpast moet je bij let userVote de volgende lijn ook aanpassen(lijn 68: op het moment van schrijven)
        }
        else {
            btns[0].classList.remove('btn-success');
            btns[0].classList.add('btn-primary');
            btns[0].textContent = 'Stem terugtrekken'
            btns[1].hidden = true
        }
        //btnFor[0].textContent = voted ? 'Stem terugtrekken' : 'Stem voor';
        //btn.classList.toggle('voted', voted);
    };

    // Functie om alle knoppen op de pagina bij te werken
    const updateAllButtons = () => {
        voteForms.forEach(form => {
            // Haal het id op van recommendation
            const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
            if (id) updateButton(form, votes[id]);
        });
    };

    // Haal bij het laden van de pagina de lijst met reeds uitgebrachte stemmen op van de server
    fetch(`/${tenant}/api/Recommendations/userVotes`)
        .then(res => res.ok ? res.json() : Promise.reject()) //bij fout gaat naar de catch
        .then((ids: string[]) => {
            // Zet voor elk ontvangen id de stemstatus op true
            ids.forEach(id => votes[id] = true);
            // Werk de knoppen bij volgens de opgehaalde data
            updateAllButtons();
        });

    // Voeg submit-handler toe aan elk stem-formulier
    voteForms.forEach(form => {
        form.querySelectorAll('button').forEach(form2 => {
            form2.addEventListener('click', async e => {
                e.preventDefault();
                const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
                if (!id) return;

                // Bepaal of we een stem trekken of uitbrengen
                const voted = votes[id];
                const url = voted
                    ? `/${tenant}/api/Recommendations/remove-vote`
                    : `/${tenant}/api/Recommendations/vote`;
                // Vind de knop en zet deze tijdelijk uit
                const btn = form.querySelector<HTMLButtonElement>('button');
                btn?.setAttribute('disabled', '');

                try {
                    // Stuur het stemverzoek naar de server
                    let userVote = {id: +id, recommended: true}
                    if (form2.textContent === 'Stem tegen') {
                        userVote.recommended = false;
                    }
                    const res = await fetch(url, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        // Stuur het id als getal
                        body: JSON.stringify(userVote)
                    });

                    // Ontvang het nieuwe stemtotaal
                    const { id: returnedId, votes: voteCount } = await res.json();
                    // Werk de teller op de pagina bij
                    const countEl = document.getElementById(`vote-count-${returnedId}`);
                    if (countEl) countEl.textContent = String(voteCount); //if statement staat er voor typescript

                    // Update in-memory stemstatus en pas knoppen aan
                    votes[id] = !voted;
                    updateAllButtons();
                } catch {
                    // Toon foutmelding bij mislukking
                    console.log('Fout bij verwerken van uw stem.');
                } finally {
                    // Zet de knop weer aan
                    btn?.removeAttribute('disabled');
                }
            })
        })
    });
});