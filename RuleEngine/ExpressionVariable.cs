using System;
using System.Collections.Generic;
using System.Text;

namespace COM.MeshStudio.Lib.Rule
{
    public class ExpressionVariable {
	private string name;
	private double value;

	public ExpressionVariable() {
		name = "";
		value = 0;
	}

	public ExpressionVariable(string name, double value) {
		this.name = name;
		this.value = value;
	}

	public string getName() {
		return name;
	}

	public double getValue() {
		return value;
	}

	public void setName(string name) {
		this.name = name;
	}

	public void setValue(double value) {
		this.value = value;
	}
}
}
