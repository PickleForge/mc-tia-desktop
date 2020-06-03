using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Declare some helpers
public enum REGISTERS {
    CPU_FLAG                = 0x00,
    CORE_FLAG               = 0x01,
    PROGRAM_COUNTER         = 0x02,
    STACK_POINTER           = 0x03,
    STACK_BASE_POINTER      = 0x04,
    SOURCE_POINTER          = 0x05, // si
    DESTINATION_POINTER     = 0x06,
    GENERAL_0               = 0x07, // ah
    GENERAL_1               = 0x08, // bh
    GENERAL_2               = 0x09, // ch
    GENERAL_3               = 0x0A, // dh
    GENERAL_4               = 0x0B, // eh
    GENERAL_5               = 0x0C,
    GENERAL_6               = 0x0D,
    GENERAL_7               = 0x0E,
    GENERAL_8               = 0x0F,
    GENERAL_9               = 0x10,
    GENERAL_A               = 0x11,
    GENERAL_B               = 0x12,
    GENERAL_C               = 0x13,
    GENERAL_D               = 0x14,
    GENERAL_E               = 0x15,
    GENERAL_F               = 0x16,
    SHARED_0                = 0x17,
    SHARED_1                = 0x18,
    SHARED_2                = 0x19,
    SHARED_3                = 0x1A,
}

// Main CPU class
public class CPU : MonoBehaviour {
    // Declare Some Shared Constants
    public const uint BOOTLOADER_LENGTH = 100;

    // Define Busses
    public Buss bus_north;
    public Buss bus_south;
    public VirtualDisplayHAL display;

    // Define components
    public Memory ram;
    public Memory disk;

    // Define CPU Cores
    protected CPUCore[] cores;

    // Define registers and such
    protected uint[] registers = new uint[] {
        0, // CPU Flag Register
        0, // Shared Register 0
        0, // Shared Register 1
        0, // Shared Register 2
        0, // Shared Register 3
    };

    private bool alive = false;

    // Events
    UnityEvent event_cpu_cycle;

    // Constructor
    public void Instantiate(CPUCore[] cores, Memory ram, Memory disk) {
        // this.bus_north = new Buss();
        // this.bus_south = new Buss();
        this.cores = cores;
        this.ram = ram;
        this.disk = disk;
        this.display = null;

        this.event_cpu_cycle = new UnityEvent();
    }

    public void Instantiate(CPUCore[] cores, Memory ram, Memory disk, VirtualDisplayHAL display) {
        // this.bus_north = new Buss();
        // this.bus_south = new Buss();
        this.cores = cores;
        this.ram = ram;
        this.disk = disk;
        this.display = display;

        // Setup event system!
        this.event_cpu_cycle = new UnityEvent();
        foreach (CPUCore core in this.cores)
            this.event_cpu_cycle.AddListener(core.cpu_clock);
    }

    // Buss Controll Methods
    public void addDeviceToNorthBuss(BussMountable device) {
        this.bus_north.addBusDevice(device);
    }

    public void addDeviceToSouthBuss(BussMountable device) {
        this.bus_south.addBusDevice(device);
    }

    public void writeToNorthBus(uint address, uint value) {
        this.bus_north.write(address, value);
    }
    public void writeToSouthBus(uint address, uint value) {
        this.bus_south.write(address, value);
    }
    public uint readFromNorthBus(uint address) {
        this.bus_north.read(address);
        return this.bus_north.buss_value;
    }
    public uint readFromSouthBus(uint address) {
        this.bus_south.read(address);
        return this.bus_south.buss_value;
    }


    // Register Access Methods
    public void writeRegister(uint address, uint value) {
        this.registers[address] = value;
    }
    public uint readRegister(uint address) {
        return this.registers[address];
    }

    public void writeRegister(REGISTERS target, uint value) {
        switch (target) {
            case REGISTERS.CPU_FLAG:
                this.registers[0] = value;
                break;
            case REGISTERS.SHARED_0:
                this.registers[1] = value;
                break;
            case REGISTERS.SHARED_1:
                this.registers[2] = value;
                break;
            case REGISTERS.SHARED_2:
                this.registers[3] = value;
                break;
            case REGISTERS.SHARED_3:
                this.registers[4] = value;
                break;
        }
    }

    public uint readRegister(REGISTERS target) {
        switch (target) {
            case REGISTERS.CPU_FLAG:
                return this.registers[0];
            case REGISTERS.SHARED_0:
                return this.registers[1];
            case REGISTERS.SHARED_1:
                return this.registers[2];
            case REGISTERS.SHARED_2:
                return this.registers[3];
            case REGISTERS.SHARED_3:
                return this.registers[4];
            default:
                return 0;
        }
    }

    // CPU Controll Methods
    private void clock_tick() {
        event_cpu_cycle.Invoke();
    }

    IEnumerator clock_thread() {
        yield return new WaitForSeconds(2f);
        while (alive) {
            // print("DISPATCHING!");
            clock_tick();
            yield return new WaitForSeconds(0f);
        }
        yield break;
    }

    void clock_thread_sync() {
        while (alive) {
            foreach (CPUCore core in cores) {
                core.cpu_clock();
            }
        }
    }

    protected void start_clock() {
        StartCoroutine(clock_thread());
    }

    public void bootup() {
        // First, load the bootloader into memory!
        uint target = BOOTLOADER_LENGTH;
        if (this.disk.getCapacity() < target)
            target = this.disk.getCapacity();
        print("Loading bootloader! Loading " + target + " bytes from disk!");
        for (uint i = 0; i < target; i++) {
            // print("Reading byte " + i + "/" + target + " for bootloader!");
            ram.write(i, disk.read(i));
        }

        // Make myself alive
        this.alive = true;

        // Now, make the first core alive!
        cores[0].alive = true;

        // Start the system clock!
        print("Starting clock!");
        start_clock();
    }

    public void stopSoft() {
        call_halt();
    }

    // Virtual System Methods
    private void vga_int_handler(CPUCore core) {
        uint task = core.readRegister(REGISTERS.GENERAL_0);
        if (task == 0x1) {
            // Set pixel!
            uint x = core.readRegister(REGISTERS.GENERAL_1);
            uint y = core.readRegister(REGISTERS.GENERAL_2);
            uint r = core.readRegister(REGISTERS.GENERAL_4);
            uint g = core.readRegister(REGISTERS.GENERAL_5);
            uint b = core.readRegister(REGISTERS.GENERAL_6);

            if (this.display != null)
                this.display.set_pixel((int) x, (int) y, (int) r, (int) g, (int) b);
        } else if (task == 0x03) {
            print("Telling them the resolution is (" + display.getWidth() + "," + display.getHeight() + ")!");

            core.writeRegister(REGISTERS.GENERAL_1, (uint)display.getWidth());
            core.writeRegister(REGISTERS.GENERAL_2, (uint)display.getHeight());
        }
    }

    public void call_interrupt(CPUCore core, uint address) {
        print("INTERRUPT " + address + " called!");
        if (address == 0x10) {
            // VGA!
            vga_int_handler(core);
        }
    }

    public void call_halt() {
        print("HALT CALLED!");
        foreach (CPUCore core in cores) {
            core.alive = false;
        }
        this.alive = false;
    }
}
