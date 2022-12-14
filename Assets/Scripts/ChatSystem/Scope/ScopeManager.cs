using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeManager : MonoBehaviour
{
    public class Scope
    {
        public uint scope {get; private set;}
        public string name {get; private set;}

        public bool editable {get; private set;}

        public static Scope Everyone {get => new Scope(0, "Everyone");}

        public Scope(uint scope, string name, bool editable = true)
        {
            this.editable = editable;
            this.scope = scope;
            this.name = name;
        }

        public Scope(string name, Scope[] scopes, bool editable = true)
        {
            this.editable = editable;
            uint newScope = scopes[0].scope;
            for (int i = 1; i < scopes.Length; i++)
            {
                newScope |= scopes[i].scope;
            }
        }

        public Scope(Scope from)
        {
            this.editable = from.editable;
            this.scope = from.scope;
            this.name = from.name;
        }

        public void AddScope(Scope add)
        {
            if(!editable)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.scope |= add.scope;
        }

        public void RemoveScope(Scope remove)
        {
            if (!editable)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.scope = ((((~remove.scope) | this.scope) & remove.scope) ^ scope);
        }

        public bool CheckAgainst(Scope against)
        {
            return (this.scope & against.scope) == against.scope;
        }
    }

    private Dictionary<uint, Scope> _scopes = new Dictionary<uint, Scope>();
    private Dictionary<string, Scope> _scopesName = new Dictionary<string, Scope>();

    private ushort _scopeNumber = 0;

    public void RegisterScope(string name)
    {
        if(_scopeNumber >= 31)
        {
            Debug.LogError("You reached the maximal amount of scopes. You migth want to use extended scope for some functionnality");
            return;
        }
        AddScope(new Scope(((uint)1) << _scopeNumber, name, false));
        _scopeNumber++;
    }

    public void RegisterExtendedScope(string name, Scope[] scopes)
    {
        Scope scope = new Scope(name, scopes, false);
        AddScope(scope);
    }

    private bool AddScope(Scope scope)
    {
        if(_scopesName.ContainsKey(scope.name))
        {
            Debug.LogError($"A scope with the name {scope.name} already exists");
            return false;
        }
        if (_scopes.TryGetValue(scope.scope, out Scope foundScope))
        {
            Debug.LogError($"The scope ({name}) you are trying to define already exists ({foundScope.name}).");
            return false;
        }
        _scopes.Add(scope.scope, scope);
        _scopesName.Add(scope.name, scope);
        return true;
    }

    public Scope GetScope(string name)
    {
        if (!_scopesName.TryGetValue(name, out Scope foundScope))
        {
            Debug.LogError($"The scope ({name}) does not exist.");
            return null;
        }
        return foundScope;
    }

    public Scope GetScope(uint value)
    {
        if (!_scopes.TryGetValue(value, out Scope foundScope))
        {
            Debug.LogError($"The scope ({value}) does not exist.");
            return null;
        }
        return foundScope;
    }
}
