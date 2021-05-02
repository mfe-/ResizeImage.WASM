function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
function openFilepicker() {
    document.getElementById("filepicker").click();
}
function showLoadingIndicator(currentfile, totalItems, proccessedItems) {
    document.getElementById("processindicator").textContent = "Proccessing " + currentfile + " " + proccessedItems + "/" + totalItems;
}
function closeLoadingIndicator() {
    document.getElementById("processindicator").textContent = "";
}
//# sourceMappingURL=file.js.map