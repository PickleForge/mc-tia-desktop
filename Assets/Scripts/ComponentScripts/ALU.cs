using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALU : MonoBehaviour {
    // Some Constants!
    private const uint msb_uint_mask = ((uint) 1 << 31);

    // Object Attributes
    private CPUCore parent = null;

    // Constructor
    public void Instantiate(CPUCore parent) {
        this.parent = parent;
    }

    protected bool isNegitive(uint n) {
        return ((n & msb_uint_mask) > 0);
    }

    // Comparison Methods
    public void compare(uint a) {
        this.parent.clearAllFlags();
        if (isNegitive(a)) {
            this.parent.setFlag(CORE_FLAGS.NEGITIVE);
        } else if (a == 0) {
            this.parent.setFlag(CORE_FLAGS.ZERO);
        }
    }

    public void compare(uint a, uint b) {
        string line = "Comparing " + a + " and " + b + "!";
        this.parent.clearAllFlags();
        this.parent.setFlag(CORE_FLAGS.TWO_COMPARED);
        if (isNegitive(a) || isNegitive(b)) {
            line += " One is NEG!";
            this.parent.setFlag(CORE_FLAGS.NEGITIVE);
        }
        if (a == 0 || b == 0) {
            line += " One is ZERO!";
            this.parent.setFlag(CORE_FLAGS.ZERO);
        }
        if (a == b) {
            line += " Equal!";
            this.parent.setFlag(CORE_FLAGS.EQUAL);
        } else if (a > b) {
            line += " Greater than!";
            this.parent.setFlag(CORE_FLAGS.GREATER_THAN);
        } else {
            line += " Less than!";
            this.parent.setFlag(CORE_FLAGS.LESS_THAN);
        }
        print(line);
    }

    // Improved Comparison Methods
    public void load(uint a) {
        compare(a);
        runcals(a, ~((uint) 0));
    }

    public void load(uint a, uint b) {
        compare(a, b);
        runcals(a, b);
    }

    private void runcals(uint a, uint b) {
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.ADD_VALUE] = add(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.SUBTRACT_VALUE] = sub(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.MULTIPLY_VALUE] = mul(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.POWER_VALUE] = pow(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.AND_VALUE] = and(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.OR_VALUE] = or(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.NOR_VALUE] = nor(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.XOR_VALUE] = xor(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.NOT_A_VALUE] = not(a);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.NOT_B_VALUE] = not(b);
        (uint, uint) dr = div_f(a, b);
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.DIVIDE_VALUE] = dr.Item1;
        this.parent.protected_registers[(int)PROTECTED_REGISTERS.REMAINDER_VALUE] = dr.Item2;
    }

    // Maths Methods
    public uint add(uint a, uint b) {
        return a + b;
    }

    public uint sub(uint a, uint b) {
        return a - b;
    }

    public uint mul(uint a, uint b) {
        return a * b;
    }

    public (uint, uint) div_f(uint a, uint b) {
        uint remainder = a % b;
        uint value = (a - remainder) / b;
        return (value, remainder);
    }

    public uint div(uint a, uint b) {
        return div_f(a, b).Item1;
    }

    public uint pow(uint a, uint b) {
        return a ^ b;
    }

    public uint and(uint a, uint b) {
        return a & b;
    }

    public uint or(uint a, uint b) {
        return a | b;
    }

    public uint nor(uint a, uint b) {
        return (~a) & (~b);
    }

    public uint xor(uint a, uint b) {
        return ((a | b) & (~(a & b)));
    }

    public uint not(uint a) {
        return ~a;
    }

    public uint rshift(uint a, int b) {
        return a >> b;
    }
    public uint lshift(uint a, int b) {
        return a << b;
    }

    public uint rshift_carry(uint a, int b) {
        //TODO carry!
        return a >> b;
    }
    public uint lshift_carry(uint a, int b) {
        //TODO carry!
        return a << b;
    }

}
