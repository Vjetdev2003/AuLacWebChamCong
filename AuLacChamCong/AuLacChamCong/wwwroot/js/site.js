window.showModal = (modalId) => {
    var modal = new bootstrap.Modal(document.getElementById(modalId), { backdrop: 'static' });
    modal.show();
};

window.hideModal = (modalId) => {
    var modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) {
        modal.hide();
    }
};

window.downloadCsv = (csvContent, fileName) => {
    try {
        // Create a Blob with the CSV content
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        // Create a temporary URL for the Blob
        const url = window.URL.createObjectURL(blob);
        // Create a temporary <a> element to trigger the download
        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        // Clean up
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        console.log('CSV download initiated:', fileName);
    } catch (error) {
        console.error('Error in downloadCsv:', error);
        throw error; // Re-throw to let Blazor handle the error
    }
};

window.showModal = (modalId) => {
    var modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
};

window.hideModal = (modalId) => {
    var modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    modal.hide();
};

window.getCurrentPosition = async () => {
    return new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition(
            position => resolve({ latitude: position.coords.latitude, longitude: position.coords.longitude }),
            error => reject(error)
        );
    });
};