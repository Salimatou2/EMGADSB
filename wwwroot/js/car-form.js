document.addEventListener('DOMContentLoaded', function () {
    // Vérifiez les ID réels de vos sélecteurs dans le formulaire
    const makeSelect = document.getElementById('CarMakeId');
    const modelSelect = document.getElementById('CarModelId');

    if (makeSelect && modelSelect) {
        makeSelect.addEventListener('change', function () {
            const makeId = this.value;
            console.log("Marque sélectionnée: " + makeId); // Pour déboguer

            if (makeId) {
                // Désactiver le sélecteur de modèle pendant le chargement
                modelSelect.disabled = true;
                modelSelect.innerHTML = '<option value="">Chargement...</option>';

                // Appel Ajax pour récupérer les modèles
                fetch(`/Cars/GetModelsByMake?makeId=${makeId}`)
                    .then(response => response.json())
                    .then(data => {
                        console.log("Modèles reçus:", data); // Pour déboguer

                        // Vider et reconstruire le sélecteur de modèles
                        modelSelect.innerHTML = '<option value="">-- Sélectionnez un modèle --</option>';

                        data.forEach(model => {
                            const option = document.createElement('option');
                            option.value = model.id;
                            option.textContent = model.name;
                            modelSelect.appendChild(option);
                        });

                        // Réactiver le sélecteur
                        modelSelect.disabled = false;
                    })
                    .catch(error => {
                        console.error("Erreur lors du chargement des modèles:", error);
                        modelSelect.innerHTML = '<option value="">Erreur de chargement</option>';
                        modelSelect.disabled = false;
                    });
            } else {
                // Si aucune marque n'est sélectionnée
                modelSelect.innerHTML = '<option value="">-- Sélectionnez d\'abord une marque --</option>';
                modelSelect.disabled = true;
            }
        });
    } else {
        console.error("Éléments de formulaire non trouvés!");
    }
});