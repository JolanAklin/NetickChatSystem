using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetickChatSystem
{
    public class ScopeManager : MonoBehaviour
    {
        private Dictionary<uint, Scope> _scopes = new Dictionary<uint, Scope>();
        private Dictionary<string, Scope> _scopesName = new Dictionary<string, Scope>();

        private ushort _scopeNumber = 0;

        private void Awake() {
            AddScope(Scope.Everyone);
        }

        public void RegisterScope(string name, Scope.ForeignReceivePolicy foreignReceivePolicy = Scope.ForeignReceivePolicy.forbidden)
        {
            if(_scopeNumber >= 31)
            {
                Debug.LogError("You reached the maximal amount of scopes. You migth want to use extended scope for some functionnality");
                return;
            }
            AddScope(new Scope(((uint)1) << _scopeNumber, name, false, foreignReceivePolicy));
            _scopeNumber++;
        }

        public void RegisterExtendedScope(string name, Scope[] scopes, Scope.CheckPolicy checkPolicy, Scope.ForeignReceivePolicy foreignReceivePolicy)
        {
            Scope scope = new Scope(name, scopes, false, checkPolicy, foreignReceivePolicy);
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
                Debug.LogError($"The scope ({scope.name}) you are trying to define already exists ({foundScope.name}).");
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
}