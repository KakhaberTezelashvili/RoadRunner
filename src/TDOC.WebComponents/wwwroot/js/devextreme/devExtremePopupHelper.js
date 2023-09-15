const templateMarkup = document.createElement("div");
templateMarkup.innerHTML =
  `<template id="popupContentTemplate">
    <style>
      .h-100 {
        height: 100%;
      }
    .popup-slot-container {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }
    </style>
    <div class="h-100 popup-slot-container">
      <slot name="BodyTemplate"></slot>
      <slot name="FooterTemplate"></slot>
    </div>
  </template>
  <div id="popupContainerTemplate">
    <div class="h-100"></div>
  </div>`;

customElements.define("td-dx-popup", class DxPopup extends HTMLElement {

  constructor() {
    super();
    // Template for popup content (include body and footer).
    const popupContentTemplate = templateMarkup.firstElementChild; // document.getElementById("popupContentTemplate");
    const popupContent = popupContentTemplate.content;
    // Create a shadow root for element.
    this.attachShadow({ mode: 'open' }).appendChild(popupContent.cloneNode(true));

    // Template for popup container.
    const popupContainerTemplate = templateMarkup.lastElementChild; // document.getElementById("popupContainerTemplate");
    const popupContainer = popupContainerTemplate.cloneNode(true);
    document.body.appendChild(popupContainer);
    this.popupContainer = popupContainer;

    this.contentContainer = popupContainer.lastElementChild;
    // StopPropagation is required to allow mouse vertical scrolling in grid(s).
    $(this.contentContainer).on('dxmousewheel', function (e) { e.stopPropagation(); });

    // Define properties.
    this.getTitle = this.getAttribute.bind(this, ["data-dx-title"]);
    this.getWidth = this.getAttribute.bind(this, ["data-dx-width"]);
    this.getHeight = this.getAttribute.bind(this, ["data-dx-height"]);

    // Define events.
    this.onHiding = this.onPopupClosing.bind(this);
    this.onShown = this.onShown.bind(this);
    this.onResize = this.onPopupResize.bind(this);
    this.onResizeEnd = this.onPopupResizeEnd.bind(this);

    this.allowDispose = false;
    this.initialized = false;
  }

  onShown() {
    this.dispatchDxEvent("popupshown");
    this.allowDispose = true;
  }

  dispatchDxEvent(name, value) {
    var event = new Event("dx" + name, { bubbles: true });
    if (value !== undefined)
      event.value = value;
    this.dispatchEvent(event);
  }

  onPopupClosing(e) {
    e.cancel = true;
    this.dispatchDxEvent("popupclosing");
  }

  onPopupResize(e) {
    this.dispatchDxEvent("popupresize");
  }

  onPopupResizeEnd(e) {
    const popup = e.component;
    const width = popup.option("width");
    const height = popup.option("height");
  }

  connectedCallback() {
    if (!this.initialized) {
      this.initialized = true;
      this.contentContainer.appendChild(this);
        $(this.popupContainer).dxPopup({
        focusStateEnabled: false,
        title: this.getTitle,
        visible: true,
        resizeEnabled: true,
        showCloseButton: false,
        closeOnOutsideClick: false,
        width: this.getWidth,
        height: this.getHeight,
        onShown: this.onShown,
        onHiding: this.onHiding,
        onResize: this.onResize,
        onResizeEnd: this.onResizeEnd
      });
    }
  }

  disconnectedCallback() {
    if (this.allowDispose) {
      this.allowDispose = false;
      $(this.popupContainer).dxPopup("dispose");
      this.popupContainer.remove();
    }
  }

});