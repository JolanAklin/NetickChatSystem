/*


MIT License

Copyright (c) 2022 Jolan Aklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

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

        public static ScopeManager Instance {get; private set;}

        private void Awake() {
            AddScope(Scope.Everyone);

            if(Instance == null)
                Instance = this;
            else
            {
                Debug.LogWarning("There is more than one ScopeManager. Destroying.");
                Destroy(this);
            }
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

        public void RegisterExtendedScope(string name, string[] scopesName, Scope.CheckPolicy checkPolicy, Scope.ForeignReceivePolicy foreignReceivePolicy)
        {
            Scope scope = new Scope(name, scopesName, false, checkPolicy, foreignReceivePolicy);
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