using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helpers
public enum CORE_FLAGS {
    EQUAL = 0,
    GREATER_THAN = 1,
    LESS_THAN = 2,
    CARRY = 3,
    NEGITIVE = 4,
    ZERO = 5,
    OVERFLOW = 6,
    TWO_COMPARED = 7
}

public enum PROTECTED_REGISTERS {
    ADD_VALUE,
    SUBTRACT_VALUE,
    MULTIPLY_VALUE,
    DIVIDE_VALUE,
    REMAINDER_VALUE,
    POWER_VALUE,
    MODULO_VALUE,
    AND_VALUE,
    OR_VALUE,
    NOR_VALUE,
    XOR_VALUE,
    NOT_A_VALUE,
    NOT_B_VALUE
}

// Main Class
public class CPUCore : MonoBehaviour {
    // Components
    private CPU parent;
    private ALU alu;
    private FPU fpu;

    // Registers
    protected uint[] registers = new uint[] {
        0, // Flag Register             00:01
        0, // Program Counter           01:02
        0, // Stack Pointer             02:03
        0, // Stack Base Pointer        03:04
        0, // Source Pointer            04:05
        0, // Destination Pointer       05:06
        0, // General Register 0        06:07
        0, // General Register 1        07:08
        0, // General Register 2        08:09
        0, // General Register 3        09:0A
        0, // General Register 4        0A:0B
        0, // General Register 5        0B:0C
        0, // General Register 6        0C:0D
        0, // General Register 7        0D:0E
        0, // General Register 8        0E:0F
        0, // General Register 9        0F:10
        0, // General Register A        10:11
        0, // General Register B        11:12
        0, // General Register C        12:13
        0, // General Register D        13:14
        0, // General Register E        14:15
        0, // General Register F        15:16
    };

    public uint[] protected_registers = new uint[] {
        0, // AV
        0, // SV
        0, // MV
        0, // DV
        0, // RV
        0, // PV
        0, // MV
        0, // AND
        0, // OR
        0, // XOR
        0, // NOR
        0, // NOT_A
        0, // NOT_B
    };

    public bool alive = false;

    // Constructor
    public CPUCore() {
        this.parent = null;
        this.alu = null;
        this.fpu = null;
    }

    public CPUCore(CPU parent, ALU alu, FPU fpu) {
        this.alu = alu;
        this.fpu = fpu;
        setParent(parent);
    }

    public void setParent(CPU parent) {
        this.parent = parent;
        if (this.parent != null) {
            this.writeRegister(REGISTERS.STACK_BASE_POINTER, this.parent.ram.getCapacity());
            this.writeRegister(REGISTERS.STACK_POINTER, this.parent.ram.getCapacity());
        }
    }

    public void setALU(ALU alu) {
        this.alu = alu;
    }

    public void setFPU(FPU fpu) {
        this.fpu = fpu;
    }

    // Register Methods
    public uint readRegister(uint address) {
        if (address >= 0x01 && address <= 0x16) {
            // Core level
            return this.registers[address - 1];
        } else {
            // CPU level
            if (address == 0x00) {
                return this.parent.readRegister((uint) 0x00);
            } else {
                return this.parent.readRegister(address - 0x16);
            }
        }
    }

    public void writeRegister(uint address, uint value) {
        if (address >= 0x01 && address <= 0x16) {
            // Core level
            this.registers[address - 1] = value;
        } else {
            // CPU level
            if (address == 0x00) {
                this.parent.writeRegister((uint) 0x00, value);
            } else {
                this.parent.writeRegister(address - 0x16, value);
            }
        }
    }

    public uint readRegister(REGISTERS target) {
        switch(target) {
            case REGISTERS.CPU_FLAG:            return parent.readRegister(target);
            case REGISTERS.SHARED_0:            return parent.readRegister(target);
            case REGISTERS.SHARED_1:            return parent.readRegister(target);
            case REGISTERS.SHARED_2:            return parent.readRegister(target);
            case REGISTERS.SHARED_3:            return parent.readRegister(target);
            case REGISTERS.CORE_FLAG:           return this.registers[0x00];
            case REGISTERS.PROGRAM_COUNTER:     return this.registers[0x01];
            case REGISTERS.STACK_POINTER:       return this.registers[0x02];
            case REGISTERS.STACK_BASE_POINTER:  return this.registers[0x03];
            case REGISTERS.SOURCE_POINTER:      return this.registers[0x04];
            case REGISTERS.DESTINATION_POINTER: return this.registers[0x05];
            case REGISTERS.GENERAL_0:           return this.registers[0x06];
            case REGISTERS.GENERAL_1:           return this.registers[0x07];
            case REGISTERS.GENERAL_2:           return this.registers[0x08];
            case REGISTERS.GENERAL_3:           return this.registers[0x09];
            case REGISTERS.GENERAL_4:           return this.registers[0x0A];
            case REGISTERS.GENERAL_5:           return this.registers[0x0B];
            case REGISTERS.GENERAL_6:           return this.registers[0x0C];
            case REGISTERS.GENERAL_7:           return this.registers[0x0D];
            case REGISTERS.GENERAL_8:           return this.registers[0x0E];
            case REGISTERS.GENERAL_9:           return this.registers[0x0F];
            case REGISTERS.GENERAL_A:           return this.registers[0x10];
            case REGISTERS.GENERAL_B:           return this.registers[0x11];
            case REGISTERS.GENERAL_C:           return this.registers[0x12];
            case REGISTERS.GENERAL_D:           return this.registers[0x13];
            case REGISTERS.GENERAL_E:           return this.registers[0x14];
            case REGISTERS.GENERAL_F:           return this.registers[0x15];
            default:                            return 0;
        }
    }

    public void writeRegister(REGISTERS target, uint value) {
        switch(target) {
            case REGISTERS.CPU_FLAG:            parent.writeRegister(target, value); break;
            case REGISTERS.SHARED_0:            parent.writeRegister(target, value); break;
            case REGISTERS.SHARED_1:            parent.writeRegister(target, value); break;
            case REGISTERS.SHARED_2:            parent.writeRegister(target, value); break;
            case REGISTERS.SHARED_3:            parent.writeRegister(target, value); break;
            case REGISTERS.CORE_FLAG:           this.registers[0x00] = value; break;
            case REGISTERS.PROGRAM_COUNTER:     this.registers[0x01] = value; break;
            case REGISTERS.STACK_POINTER:       this.registers[0x02] = value; break;
            case REGISTERS.STACK_BASE_POINTER:  this.registers[0x03] = value; break;
            case REGISTERS.SOURCE_POINTER:      this.registers[0x04] = value; break;
            case REGISTERS.DESTINATION_POINTER: this.registers[0x05] = value; break;
            case REGISTERS.GENERAL_0:           this.registers[0x06] = value; break;
            case REGISTERS.GENERAL_1:           this.registers[0x07] = value; break;
            case REGISTERS.GENERAL_2:           this.registers[0x08] = value; break;
            case REGISTERS.GENERAL_3:           this.registers[0x09] = value; break;
            case REGISTERS.GENERAL_4:           this.registers[0x0A] = value; break;
            case REGISTERS.GENERAL_5:           this.registers[0x0B] = value; break;
            case REGISTERS.GENERAL_6:           this.registers[0x0C] = value; break;
            case REGISTERS.GENERAL_7:           this.registers[0x0D] = value; break;
            case REGISTERS.GENERAL_8:           this.registers[0x0E] = value; break;
            case REGISTERS.GENERAL_9:           this.registers[0x0F] = value; break;
            case REGISTERS.GENERAL_A:           this.registers[0x10] = value; break;
            case REGISTERS.GENERAL_B:           this.registers[0x11] = value; break;
            case REGISTERS.GENERAL_C:           this.registers[0x12] = value; break;
            case REGISTERS.GENERAL_D:           this.registers[0x13] = value; break;
            case REGISTERS.GENERAL_E:           this.registers[0x14] = value; break;
            case REGISTERS.GENERAL_F:           this.registers[0x15] = value; break; ;
        }
    }

    /*
     * Flags:
     *  0: Equal
     *  1: Greater Than
     *  2: Less Than
     *  3: Carry
     *  4: Negitive
     *  5: Zero
     *  6: Overflow
     *  7: Two Compared
     * 
     */

    public void setFlag(int index) {
        int mask = (1 << index);
        this.registers[0] |= (uint)mask;
    }

    public void clearFlag(int index) {
        int mask = (1 << index);
        this.registers[0] &= (uint)~mask;
    }

    public bool readFlag(int index) {
        // print("Current flag value: " + this.registers[0]);
        uint value = (this.registers[0] >> index) & 1;
        return (value == 1);
    }

    public void setFlag(CORE_FLAGS index) { setFlag((int)index); }
    public void clearFlag(CORE_FLAGS index) { setFlag((int)index); }
    public bool readFlag(CORE_FLAGS index) { return readFlag((int)index); }

    public void clearAllFlags() {
        this.registers[0] = 0;
    }

    // Clock Responder
    public void cpu_clock() {
        if (this.alive)
            cycle();
    }

    private void cycle() {
        if (this.alive) {
            // print("CPU_CLOCK_TICK");
            uint address = readRegister(REGISTERS.PROGRAM_COUNTER); // Get the program counter
            uint next_address = execute(address);
            writeRegister(REGISTERS.PROGRAM_COUNTER, next_address);

            /*
            print("REGISTERS: ");
            string line = "";
            for (int i = 0; i < this.registers.Length; i++) {
                line = line + registers[i] + ",";
            }
            print("(" + line + ")");
            */
            
        } else {
            // print("ILLEGAL_CLOCK_TICK");
        }
    }

    // Virtual Architecture Methods
    // Internal Tools
    private void write(uint address, uint value) { this.parent.ram.write(address, value); }
    private uint read(uint address) { return this.parent.ram.read(address); }

    private void stack_push(uint value) {
        uint address = readRegister(REGISTERS.STACK_POINTER);
        write(address, value);
        writeRegister(REGISTERS.STACK_POINTER, address - 1);
    }

    public uint stack_pop() {
        uint address = readRegister(REGISTERS.STACK_POINTER) + 1;
        if (address == readRegister(REGISTERS.STACK_BASE_POINTER)) {
            // There isn't anything in the stack!
            return 0;
        }
        writeRegister(REGISTERS.STACK_POINTER, address);
        return read(address);
    }

    // Execution Method
    private uint execute(uint address) {
        if (!alive) {
            // print("I'm supposed to be dead!");
            return 0;
        };

        uint instruction = read(address);
        print("Executing instruction (" + instruction + ") @ " + address + "!");

        uint v1, v2 = 0;

        switch (instruction) {
            default: return address + 1;
            case 0x00: return address + 1;

            case 0x01: // INT_L
                parent.call_interrupt(this, read(address + 1));
                return address + 2;
            case 0x02: // INT_R
                parent.call_interrupt(this, readRegister(read(address + 1)));
                return address + 2;
            case 0x03: // MOV_RL
                writeRegister(read(address + 1), read(address + 2));
                return address + 3;
            case 0x04: // MOV_RR
                writeRegister(read(address + 1), readRegister(read(address + 2)));
                return address + 3;
            case 0x07: // COMP_LL
                alu.compare(read(address + 1), read(address + 2));
                return address + 3;
            case 0x08: // COMP_RL
                alu.compare(this.readRegister(read(address + 1)), read(address + 2));
                return address + 3;
            case 0x09: // COMP_RR
                alu.compare(this.readRegister(read(address + 1)), this.readRegister(read(address + 2)));
                return address + 3;
            case 0x0A: // JEQ_L
                print("[JEQ_L] Attempting branch to " + this.read(address + 1) + "! " + ((this.readFlag(CORE_FLAGS.EQUAL)) ? "OK" : "FAIL") );
                if (this.readFlag(CORE_FLAGS.EQUAL))
                    return this.read(address + 1);
                return address + 2;
            case 0x0B: // JEQ_R
                print("[JEQ_R] Attempting branch to " + this.readRegister(this.read(address + 1)) + "! " + ((this.readFlag(CORE_FLAGS.EQUAL)) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.EQUAL))
                    return this.readRegister(this.read(address + 1));
                return address + 2;
            case 0x0C: // JLT_L
                print("[JLT_L] Attempting branch to " + this.read(address + 1) + "! " + ((this.readFlag(CORE_FLAGS.LESS_THAN)) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.LESS_THAN))
                    return this.read(address + 1);
                return address + 2;
            case 0x0D: // JLT_R
                print("[JLT_R] Attempting branch to " + this.readRegister(this.read(address + 1)) + "! " + ((this.readFlag(CORE_FLAGS.LESS_THAN)) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.LESS_THAN))
                    return this.readRegister(this.read(address + 1));
                return address + 2;
            case 0x0E: // JGT_L
                print("[JGT_L] Attempting branch to " + this.read(address + 1) + "! " + ((this.readFlag(CORE_FLAGS.GREATER_THAN)) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.GREATER_THAN))
                    return this.read(address + 1);
                return address + 2;
            case 0x0F: // JGT_R
                print("[JGT_R] Attempting branch to " + this.readRegister(this.read(address + 1)) + "! " + ((this.readFlag(CORE_FLAGS.GREATER_THAN)) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.GREATER_THAN))
                    return this.readRegister(this.read(address + 1));
                return address + 2;
            case 0x10: // JNEQ_L
                print("[JNEQ_L] Attempting branch to " + this.read(address + 1) + "! " + ((this.readFlag(CORE_FLAGS.EQUAL)) ? "FAIL" : "OK"));
                if (!this.readFlag(CORE_FLAGS.EQUAL))
                    return this.read(address + 1);
                return address + 2;
            case 0x11: // JNEQ_R
                print("[JNEQ_R] Attempting branch to " + this.readRegister(this.read(address + 1)) + "! " + ((this.readFlag(CORE_FLAGS.EQUAL)) ? "FAIL" : "OK"));
                if (!this.readFlag(CORE_FLAGS.EQUAL))
                    return this.readRegister(this.read(address + 1));
                return address + 2;
            case 0x12: // ADD_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.add(v1, v2));
                return address + 3;
            case 0x13: // ADD_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.add(v1, v2));
                return address + 3;
            case 0x14: // SUB_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.sub(v1, v2));
                return address + 3;
            case 0x15: // SUB_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.sub(v1, v2));
                return address + 3;
            case 0x16: // MUL_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.mul(v1, v2));
                return address + 3;
            case 0x17: // MUL_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.mul(v1, v2));
                return address + 3;
            case 0x18: // DIV_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.div(v1, v2));
                return address + 3;
            case 0x19: // DIV_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.div(v1, v2));
                return address + 3;
            case 0x1A: // POW_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.pow(v1, v2));
                return address + 3;
            case 0x1B: // POW_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.pow(v1, v2));
                return address + 3;
            case 0x1C: // AND_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.and(v1, v2));
                return address + 3;
            case 0x1D: // AND_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.and(v1, v2));
                return address + 3;
            case 0x1E: // OR_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.or(v1, v2));
                return address + 3;
            case 0x1F: // OR_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.or(v1, v2));
                return address + 3;
            case 0x20: // NOR_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.nor(v1, v2));
                return address + 3;
            case 0x21: // NOR_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.nor(v1, v2));
                return address + 3;
            case 0x22: // XOR_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.xor(v1, v2));
                return address + 3;
            case 0x23: // XOR_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.xor(v1, v2));
                return address + 3;
            case 0x24: // NOT_R
                v1 = this.readRegister(this.read(address + 1));
                this.writeRegister(read(address + 1), this.alu.not(v1));
                return address + 2;
            case 0x25: // HALT
                parent.call_halt();
                return address + 1;
            case 0x26: // LSHIFT_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.lshift(v1, (int)v2));
                return address + 3;
            case 0x27: // LSHIFT_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.lshift(v1, (int) v2));
                return address + 3;
            case 0x2A: // RSHIFT_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.rshift(v1, (int) v2));
                return address + 3;
            case 0x2B: // RSHIFT_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.rshift(v1, (int)v2));
                return address + 3;
            case 0x2E: // JGTEQ_L
                print("[JGTEQ_L] Attempting branch to " + this.read(address + 1) + "! " + (((this.readFlag(CORE_FLAGS.GREATER_THAN) || this.readFlag(CORE_FLAGS.EQUAL))) ? "OK" : "FAIL"));

                if (this.readFlag(CORE_FLAGS.GREATER_THAN) || this.readFlag(CORE_FLAGS.EQUAL))
                    return read(address + 1);
                return address + 2;
            case 0x2F: // JGTEQ_R
                print("[JGTEQ_R] Attempting branch to " + readRegister(read(address + 1)) + "! " + (((this.readFlag(CORE_FLAGS.GREATER_THAN) || this.readFlag(CORE_FLAGS.EQUAL))) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.GREATER_THAN) || this.readFlag(CORE_FLAGS.EQUAL))
                    return readRegister(read(address + 1));
                return address + 2;
            case 0x30: // JLTEQ_L
                print("[JLTEQ_L] Attempting branch to " + this.read(address + 1) + "! " + (((this.readFlag(CORE_FLAGS.LESS_THAN) || this.readFlag(CORE_FLAGS.EQUAL))) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.LESS_THAN) || this.readFlag(CORE_FLAGS.EQUAL))
                    return read(address + 1);
                return address + 2;
            case 0x31: // JLTEQ_R
                print("[JLTEQ_R] Attempting branch to " + readRegister(read(address + 1)) + "! " + (((this.readFlag(CORE_FLAGS.LESS_THAN) || this.readFlag(CORE_FLAGS.EQUAL))) ? "OK" : "FAIL"));
                if (this.readFlag(CORE_FLAGS.LESS_THAN) || this.readFlag(CORE_FLAGS.EQUAL))
                    return readRegister(read(address + 1));
                return address + 2;
            case 0x32: // LSHIFT_CARRY_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.lshift_carry(v1, (int) v2));
                return address + 3;
            case 0x33: // LSHIFT_CARRY_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.lshift_carry(v1, (int)v2));
                return address + 3;
            case 0x36: // RSHIFT_CARRY_RL
                v1 = this.readRegister(this.read(address + 1));
                v2 = read(address + 2);
                this.writeRegister(read(address + 1), this.alu.rshift_carry(v1, (int)v2));
                return address + 3;
            case 0x37: // RSHIFT_CARRY_RR
                v1 = this.readRegister(this.read(address + 1));
                v2 = this.readRegister(read(address + 2));
                this.writeRegister(read(address + 1), this.alu.rshift_carry(v1, (int)v2));
                return address + 3;
            case 0x3A: // JMP_L
                return read(address + 1);
            case 0x3B: // JMP_R
                return readRegister(read(address + 1));
            case 0x3C: // CALL_L
                stack_push(address);
                return read(address + 1);
            case 0x3D: // CALL_R
                stack_push(address);
                return readRegister(read(address + 1));
            case 0x3E: // RET
                return stack_pop();
            case 0x40: // LDM_RL
                v1 = read(address + 2);
                writeRegister(read(address + 1), read(v1));
                return address + 3;
            case 0x41: // LDM_RR
                v1 = readRegister(read(address + 2));
                writeRegister(read(address + 1), read(v1));
                return address + 3;
            // FOR SVM: Second argument is what to write, first argument is address!
            case 0x42: // SVM_RL
                v1 = readRegister(read(address + 1));
                v2 = read(address + 2);
                write(v1, v2);
                return address + 3;
            case 0x43: // SVM_RR
                v1 = readRegister(read(address + 1));
                v2 = readRegister(read(address + 2));
                write(v1, v2);
                return address + 3;
            case 0x44: // SVM_LL
                v1 = read(address + 1);
                v2 = read(address + 2);
                write(v1, v2);
                return address + 3;
            case 0x45: // SVM_LR
                v1 = read(address + 1);
                v2 = readRegister(read(address + 2));
                write(v1, v2);
                return address + 3;
        }
    }

}
