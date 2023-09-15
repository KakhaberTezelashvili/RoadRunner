window.scannerSimulator = {
    _scannerSimulatorInvokeHelper: null,

    scanBarcode: (barcode) => {
        if (_scannerSimulatorInvokeHelper) {
            _scannerSimulatorInvokeHelper.invokeMethodAsync('ScanBarcode', barcode);
        }
    },

    initScannerSimulator: (scannerSimulatorInvokeHelper) => {
        _scannerSimulatorInvokeHelper = scannerSimulatorInvokeHelper;
    },

    disposeScannerSimulator: () => {
        if (_scannerSimulatorInvokeHelper) {
            _scannerSimulatorInvokeHelper.dispose();
        }
    }
}