using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStates {
    private readonly NetworkConnection conn;
    private readonly int id;
    public readonly Dictionary<string, GeneralStateValue> allValues = new Dictionary<string, GeneralStateValue>(); 
    public readonly Dictionary<string, GeneralStateValue> testValues = new Dictionary<string, GeneralStateValue>(); // это не для теста, не удалять
    private readonly Dictionary<Type, PlayerState> states = new Dictionary<Type, PlayerState>();

    public NetworkConnection Conn => conn;
    public int Id => id;

    public PlayerStates(NetworkConnection conn, int id) {
        this.conn = conn;
        this.id = id;
    }

    /// <summary>
    /// Возвращает указанное состояние игрока
    /// </summary>
    /// <typeparam name="T">Тип нужного состояния</typeparam>
    public T GetState<T>() where T : PlayerState {
        if (!states.ContainsKey(typeof(T)))
            return CreateState<T>();
            
        return (T) states[typeof(T)];
    }

    public T CreateState<T>() where T : PlayerState {
        ConstructorInfo constructor = typeof(T).GetConstructor(new[] { typeof(PlayerStates), typeof(bool) });
        T state = (T) constructor.Invoke(new object[]{ this, false });
        states.Add(typeof(T), state);
        return state;
    }

    public GeneralStateValue GetStateValue(string valueName) {
        return allValues.Get(valueName);
    }
    
    public void CreateStateWithValue(string valueName) {
        List<Type> loadedStates = Players.GetLoadedStates();
        foreach (Type loadedState in loadedStates) {
            if (states.Get(loadedState) != null)
                continue;

            ConstructorInfo constructor = loadedState.GetConstructor(new[] { typeof(PlayerStates), typeof(bool) });
            if (constructor == null)
                Debug.LogError(loadedState + " must have (PlayerStates, bool) constructor");

            constructor.Invoke(new object[]{ this, true });
            if (IsStateHaveValue(valueName))
                states.Add(loadedState, (PlayerState) constructor.Invoke(new object[]{ this, false }));
            
            testValues.Clear();
        }
    }

    private bool IsStateHaveValue(string valueName) {
        return testValues.ContainsKey(valueName);
    }

    public override bool Equals(object obj) {
        return obj is PlayerStates states && states.id == id;
    }

    public override int GetHashCode() {
        return id;
    }
}
