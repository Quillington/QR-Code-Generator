using System;

public class Pointer
{
    public int Xpointer { get => _xpointer; set => _xpointer = value; }
    private int _xpointer;
    public int Ypointer { get => _ypointer; set => _ypointer = value; }
    private int _ypointer;
    public int messageIndex { get => _messageindex; set => _messageindex = value; }
    private int _messageindex;

    public Pointer() {
        _xpointer = 0;
        _ypointer = 0;
        _messageindex = 0;
    }

    public void X_left() {
        _xpointer += 2;
    }
    public void Y_up() {
        _ypointer += 4;
    }
    public void Y_down() {
        _ypointer += -4;
    }
}
