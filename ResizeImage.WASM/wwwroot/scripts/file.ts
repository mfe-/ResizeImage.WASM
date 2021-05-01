function saveAsFile(filename: string, bytesBase64: string): void {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function openFilepicker(): void {
    document.getElementById("filepicker").click();
}