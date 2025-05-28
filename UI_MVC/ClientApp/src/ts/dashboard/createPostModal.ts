declare global {
    interface Window {
        openCreatePostModal: (panelId: number | string) => void;
    }
}

export function initializeCreatePostModal(): void {
    const createPostModal = document.getElementById('createPostModal') as HTMLElement | null;
    const closeCreatePostModalBtn = document.getElementById('closeCreatePostModalBtn') as HTMLButtonElement | null;
    const cancelCreatePostBtn = document.getElementById('cancelCreatePostBtn') as HTMLButtonElement | null;
    const modalPanelIdInput = document.getElementById('modalPanelIdForPost') as HTMLInputElement | null;
    const createPostForm = document.getElementById('createPostForm') as HTMLFormElement | null;
    const titleInput = createPostModal?.querySelector('input[name="Title"]') as HTMLInputElement | null;
    const addPostButton = document.querySelector('.add-post-button') as HTMLAnchorElement | null;

    const openModal = (panelId: number | string): void => {
        if (modalPanelIdInput) {
            modalPanelIdInput.value = panelId.toString();
        }
        if (createPostModal) {
            createPostModal.classList.remove('hidden');
            titleInput?.focus();
        }
    };

    const closeModal = (): void => {
        if (createPostModal) {
            createPostModal.classList.add('hidden');
            if (createPostForm) {
                createPostForm.reset();
            }
        }
    };

    if (addPostButton) {
        addPostButton.addEventListener('click', (event) => {
            event.preventDefault();
            const panelId = addPostButton.getAttribute('data-panel-id') || '';
            openModal(panelId);
        });
    }

    if (closeCreatePostModalBtn) {
        closeCreatePostModalBtn.addEventListener('click', closeModal);
    }

    if (cancelCreatePostBtn) {
        cancelCreatePostBtn.addEventListener('click', closeModal);
    }

    // Handle form submission via AJAX
    let isSubmitting = false;
    
    if (createPostForm) {
        const submitBtn = createPostForm.querySelector('button[type="submit"]') as HTMLButtonElement;
        if (submitBtn) {
            submitBtn.addEventListener('click', async function() {
                if (isSubmitting) return;
                isSubmitting = true;
                submitBtn.disabled = true;
                
                const formData = new FormData(createPostForm);
                
                try {
                    const response = await fetch(createPostForm.action, {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    });
                    
                    const result = await response.json();
                    
                    if (result.success) {
                        closeModal();
                        window.location.reload();
                    } else if (result.errors) {
                        // Clear previous errors
                        const errorElements = createPostModal?.querySelectorAll('.modal-validation-message');
                        errorElements?.forEach(el => el.textContent = '');
                        
                        // Display new errors
                        Object.keys(result.errors).forEach(key => {
                            const errorMsg = result.errors[key][0];
                            const validationElement = createPostModal?.querySelector(`[data-valmsg-for="${key}"]`);
                            if (validationElement) validationElement.textContent = errorMsg;
                        });
                    }
                } catch (error) {
                    console.error('Error submitting form:', error);
                } finally {
                    isSubmitting = false;
                    submitBtn.disabled = false;
                }
            });
        }
    }

    document.addEventListener('keydown', (event: KeyboardEvent) => {
        if (event.key === 'Escape' && createPostModal && !createPostModal.classList.contains('hidden')) {
            closeModal();
        }
    });
    
    window.openCreatePostModal = openModal;
}

document.addEventListener('DOMContentLoaded', () => {
    initializeCreatePostModal();
});