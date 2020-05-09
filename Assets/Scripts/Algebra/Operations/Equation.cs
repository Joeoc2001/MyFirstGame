﻿using Rationals;
using System;
using System.Collections.Generic;

public abstract class Equation
{
    public abstract Func<VariableSet, float> GetExpression();

    public abstract Equation GetDerivative(Variable wrt);

    public abstract Equation GetSimplified();

    public static Addition operator +(Equation left, Equation right)
    {
        return new Addition(new Equation[] { left, right });
    }

    public static Addition operator +(Equation left, Rational right)
    {
        return left + new Constant(right);
    }

    public static Addition operator +(Rational left, Equation right)
    {
        return new Constant(left) + right;
    }

    public static Addition operator -(Equation left, Equation right)
    {
        return new Addition(new Equation[] { left, Constant.MINUS_ONE * right });
    }

    public static Addition operator -(Equation left, Rational right)
    {
        return left + new Constant(-right);
    }

    public static Addition operator -(Rational left, Equation right)
    {
        return new Constant(-left) + right;
    }

    public static Multiplication operator *(Equation left, Equation right)
    {
        return new Multiplication(new Equation[] { left, right });
    }

    public static Multiplication operator *(Equation left, Rational right)
    {
        return left * new Constant(right);
    }

    public static Multiplication operator *(Rational left, Equation right)
    {
        return new Constant(left) * right;
    }

    public static Multiplication operator /(Equation left, Equation right)
    {
        // a/b = a * (b^-1)
        return new Multiplication(new Equation[] { left, new Exponentiation(right, Constant.MINUS_ONE) });
    }

    public static Multiplication operator /(Equation left, Rational right)
    {
        return left / new Constant(right);
    }

    public static Multiplication operator /(Rational left, Equation right)
    {
        return new Constant(left) / right;
    }

    public static Exponentiation Pow(Equation left, Equation right)
    {
        return new Exponentiation(left, right);
    }

    public static Exponentiation Pow(Equation left, Rational right)
    {
        return Pow(left, new Constant(right));
    }

    public static Exponentiation Pow(Rational left, Equation right)
    {
        return Pow(new Constant(left), right);
    }

    public override abstract int GetHashCode();

    public override abstract bool Equals(object obj);

    public static bool operator ==(Equation left, Equation right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Equation left, Equation right)
    {
        return !(left == right);
    }
}
