function createObjectURL(base64Data, name, type) {
    const binaryString = atob(base64Data);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    const file = new File([bytes], name, { type: type });
    return URL.createObjectURL(file);
}

MyWorksheet.Blazor.CreateObjectURL = createObjectURL;