const NAME = 'appCulture';
window.appCulture = {
    get: () => localStorage[NAME] || 'en-US',
    set: (value) => localStorage[NAME] = value
}