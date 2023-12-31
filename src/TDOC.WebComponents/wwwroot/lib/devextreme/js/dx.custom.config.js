"use strict";

/* Comment lines below for the widgets you don't require and run "devextreme-bundler" in this directory, then include dx.custom.js in your project */
/* For more information read devexpress docs in "https://js.devexpress.com/Documentation/Guide/Common/Modularity/Create_a_Custom_Bundle/" */

/* Core (dx.module-core.js) */
/* eslint-disable import/no-commonjs */
const DevExpress = require('devextreme/bundles/modules/core');

/* Integrations (dx.module-core.js) */

require('devextreme/integration/jquery');

/* Events (dx.module-core.js) */

require('devextreme/events/click');
require('devextreme/events/contextmenu');
require('devextreme/events/double_click');
require('devextreme/events/drag');
require('devextreme/events/hold');
require('devextreme/events/hover');
require('devextreme/events/pointer');
require('devextreme/events/swipe');
require('devextreme/events/transform');

/* UI core (dx.module-core.js) */

const ui = DevExpress.ui = require('devextreme/bundles/modules/ui');

ui.themes = require('devextreme/ui/themes');

// deprecated
ui.setTemplateEngine = require('devextreme/core/templates/template_engine_registry').setTemplateEngine;

ui.dialog = require('devextreme/ui/dialog');
ui.notify = require('devextreme/ui/notify');

/* Base widgets (dx.module-widgets-base.js) */

// ui.dxActionSheet = require('devextreme/ui/action_sheet');
// ui.dxAutocomplete = require('devextreme/ui/autocomplete');
// ui.dxBox = require('devextreme/ui/box');
// ui.dxButton = require('devextreme/ui/button');
// ui.dxDropDownButton = require('devextreme/ui/drop_down_button');
// ui.dxButtonGroup = require('devextreme/ui/button_group');
// ui.dxCalendar = require('devextreme/ui/calendar');
// ui.dxCheckBox = require('devextreme/ui/check_box');
// ui.dxColorBox = require('devextreme/ui/color_box');
// ui.dxDateBox = require('devextreme/ui/date_box');
// ui.dxDrawer = require('devextreme/ui/drawer');
// ui.dxDeferRendering = require('devextreme/ui/defer_rendering');
// ui.dxDropDownBox = require('devextreme/ui/drop_down_box');
// ui.dxDropDownMenu = require('devextreme/ui/drop_down_menu');
// ui.dxFileUploader = require('devextreme/ui/file_uploader');
// ui.dxForm = require('devextreme/ui/form');
// ui.dxGallery = require('devextreme/ui/gallery');
// ui.dxHtmlEditor = require('devextreme/ui/html_editor');
// ui.dxList = require('devextreme/ui/list');
// ui.dxLoadIndicator = require('devextreme/ui/load_indicator');
// ui.dxLoadPanel = require('devextreme/ui/load_panel');
// ui.dxLookup = require('devextreme/ui/lookup');
// ui.dxMap = require('devextreme/ui/map');
// ui.dxMultiView = require('devextreme/ui/multi_view');
// ui.dxNavBar = require('devextreme/ui/nav_bar');
// ui.dxNumberBox = require('devextreme/ui/number_box');
// ui.dxOverlay = require('devextreme/ui/overlay/ui.overlay');
//ui.dxPopover = require('devextreme/ui/popover');
ui.dxPopup = require('devextreme/ui/popup');
// ui.dxProgressBar = require('devextreme/ui/progress_bar');
// ui.dxRadioGroup = require('devextreme/ui/radio_group');
// ui.dxRangeSlider = require('devextreme/ui/range_slider');
ui.dxResizable = require('devextreme/ui/resizable');
// ui.dxResponsiveBox = require('devextreme/ui/responsive_box');
// ui.dxScrollView = require('devextreme/ui/scroll_view');
// ui.dxSelectBox = require('devextreme/ui/select_box');
// ui.dxSlider = require('devextreme/ui/slider');
// ui.dxSpeedDialAction = require('devextreme/ui/speed_dial_action');
// ui.dxSwitch = require('devextreme/ui/switch');
// ui.dxTabPanel = require('devextreme/ui/tab_panel');
// ui.dxTabs = require('devextreme/ui/tabs');
// ui.dxTagBox = require('devextreme/ui/tag_box');
// ui.dxTextArea = require('devextreme/ui/text_area');
// ui.dxTextBox = require('devextreme/ui/text_box');
// ui.dxTileView = require('devextreme/ui/tile_view');
ui.dxToast = require('devextreme/ui/toast');
ui.dxToolbar = require('devextreme/ui/toolbar');
// ui.dxTooltip = require('devextreme/ui/tooltip');
// ui.dxTrackBar = require('devextreme/ui/track_bar');
// ui.dxDraggable = require('devextreme/ui/draggable');
// ui.dxSortable = require('devextreme/ui/sortable');
