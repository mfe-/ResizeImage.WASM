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
function showLoadingIndicator(currentfile: string, totalItems: number, proccessedItems: number): void {
    document.getElementById("processindicator").textContent = "Proccessing " + currentfile + " " + proccessedItems + "/" + totalItems;
}
function closeLoadingIndicator(): void {
    document.getElementById("processindicator").textContent = "";
}