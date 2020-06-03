using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour, BussMountable {
    // Attributes
    private uint[] data;
    // private readonly bool readOnly;

    private uint buss_addr_min;
    private uint buss_addr_max;

    // Constructors
    public void Instantiate(uint capacity) {
        this.data = new uint[capacity];
    }

    public void Instantiate(uint[] data) {
        this.data = data;
    }

    // Data Access Methods
    public void write(uint address, uint value) {
        this.data[address] = value;
    }
    public uint read(uint address) {
        return this.data[address];
    }

    // Buss Extender
    public void assignAddresses(uint address_floor, uint address_ceil) {
        this.buss_addr_min = address_floor;
        this.buss_addr_max = address_ceil;
    }

    public void onBusRead(Buss buss, uint address) {
        if (address >= this.buss_addr_min && address <= this.buss_addr_max) {
            buss.buss_value = this.data[address - this.buss_addr_min];
        }
    }

    public void onBusWrite(Buss buss, uint address, uint value) {
        if (address >= this.buss_addr_min && address <= this.buss_addr_max) {
            this.data[address - this.buss_addr_min] = value;
        }
    }

    public uint getCapacity() {
        // print("MEMORY CAPACITY => " + this.data.Length);
        return (uint) this.data.Length;
    }
}
