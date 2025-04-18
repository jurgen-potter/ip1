document.addEventListener('DOMContentLoaded', () => {
    // ----- Elements & Storage Key -----
    const voteForms = document.querySelectorAll<HTMLFormElement>('.vote-form');
    const userId = document.getElementById('current-user-id')?.dataset.userId ?? 'anonymous';
    const storageKey = `userVotes_${userId}`;

    // ----- Vote Map Helpers -----
    // Reads current votes from localStorage or returns empty map
    const getVotes = () =>
        JSON.parse(localStorage.getItem(storageKey) ?? '{}') as Record<string, boolean>;

    // Saves updated vote map back to localStorage
    const setVotes = (votes: Record<string, boolean>) =>
        localStorage.setItem(storageKey, JSON.stringify(votes));

    // ----- UI Updates -----
    // Toggles button text and class based on vote state
    const updateButtons = (votes: Record<string, boolean>) => {
        voteForms.forEach(form => {
            const idInput = form.querySelector<HTMLInputElement>('input[name="id"]');
            const btn = form.querySelector<HTMLButtonElement>('button');
            if (!idInput || !btn) return;

            const voted = votes[idInput.value];
            btn.textContent = voted ? 'Stem terugtrekken' : 'Stem';
            btn.classList.toggle('voted', voted);
        });
    };

    // ----- Initial Sync -----
    // Fetch server votes; on failure, fallback to localStorage
    fetch('/Recommendation/GetUserVotes')
        .then(res => res.ok ? res.json() as Promise<string[]> : Promise.reject())
        .then(ids => {
            const votes: Record<string, boolean> = {};
            ids.forEach(id => votes[id] = true);
            setVotes(votes);
            updateButtons(votes);
        })
        .catch(() => updateButtons(getVotes()));

    // ----- Vote/Unvote Handler -----
    // On form submit, send vote toggle to server, update count+UI
    voteForms.forEach(form => {
        form.addEventListener('submit', async e => {
            e.preventDefault();  // prevent page reload

            const idInput = form.querySelector<HTMLInputElement>('input[name="id"]');
            if (!idInput) return;
            const recId = idInput.value;

            const votes = getVotes();
            const removing = votes[recId];
            const url = removing ? '/Recommendation/RemoveVote' : '/Recommendation/Vote';

            const btn = form.querySelector<HTMLButtonElement>('button');
            btn?.setAttribute('disabled', '');

            try {
                const res = await fetch(url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(+recId)  //cast to number
                });

                const data = await res.json() as { id: string; votes: number };
                const countEl = document.getElementById(`vote-count-${data.id}`);
                if (countEl) countEl.textContent = String(data.votes);

                // Update local map and UI
                if (removing) delete votes[recId];
                else votes[recId] = true;
                setVotes(votes);
                updateButtons(votes);

            } catch {
                alert('Fout bij verwerken van uw stem.');
            } finally {
                btn?.removeAttribute('disabled');
            }
        });
    });
});
