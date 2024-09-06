// Get DOM elements
const fileInput = document.getElementById('fileInput');
const fileDropArea = document.getElementById('fileDropArea');

// Handle click on drop area to trigger file input
fileDropArea.addEventListener('click', () => {
    fileInput.click();
});

// Handle file input change (when files are selected)
fileInput.addEventListener('change', handleFiles);

// Handle drag events
fileDropArea.addEventListener('dragover', (event) => {
    event.preventDefault();
    fileDropArea.classList.add('dragover');
});

fileDropArea.addEventListener('dragleave', () => {
    fileDropArea.classList.remove('dragover');
});

fileDropArea.addEventListener('drop', (event) => {
    event.preventDefault();
    fileDropArea.classList.remove('dragover');

    // Get dropped files
    const files = event.dataTransfer.files;
    handleFiles({ target: { files } });
});

// Function to handle files and display them
function handleFiles(event) {
    const files = event.target.files;
    if (files.length > 0) {
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            console.log('File selected:', file.name);
            // Here you can process the file further, like displaying the file name, etc.
        }
    }
}
