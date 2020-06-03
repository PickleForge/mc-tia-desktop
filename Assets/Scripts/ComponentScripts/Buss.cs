using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BussMountable {
    void assignAddresses(uint address_floor, uint address_ceil);
    void onBusRead(Buss buss, uint address);
    void onBusWrite(Buss buss, uint address, uint value);
}

public class Buss : MonoBehaviour {
    private BussMountable[] devices;
    public uint buss_value;

    public Buss() {
        this.buss_value = 0;
        this.devices = new BussMountable[] { };
    }

    public void read(uint address) {
        for (int i = 0; i < this.devices.Length; i++) {
            this.devices[i].onBusRead(this, address);
        }
    }

    public void write(uint address, uint value) {
        for (int i = 0; i < this.devices.Length; i++) {
            this.devices[i].onBusWrite(this, address, value);
        }
    }

    public void addBusDevice(BussMountable device) {
        this.devices[this.devices.Length] = device;
    }

}
