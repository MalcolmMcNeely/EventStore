window.scrollToBottom = (elementId) => {
    let elem = document.getElementById(elementId);
    if (elem) {
        elem.scrollTop = elem.scrollHeight;
    }
};

window.scrollToTop = (elementId) => {
    let elem = document.getElementById(elementId);
    if (elem) {
        elem.scrollTop = 0;
    }
};