using System.Collections.Generic;
using Ecs.Core.Utils.TickInterfaces;
using Reflex.Attributes;
using UnityEngine;

namespace Ecs.Core.Utils
{
    public class GameLoopHandler : MonoBehaviour
    {
        private IEnumerable<IController> _controllers;
        
        private readonly List<IStartable> _startables = new ();
        private readonly List<ITickable> _tickables = new();
        private readonly List<IFixedTickable> _fixedTickables = new();
        private readonly List<ILateTickable> _lateTickables = new();
        
        [Inject]
        private void Construct(IEnumerable<IController> controllers)
        {
            _controllers = controllers;
        }
        
        private void Awake()
        {
            Debug.Log($"{nameof(SystemLoopHandler)} :: Awake");
            
            foreach (var controller in _controllers)
            {
                switch (controller)
                {
                    case IStartable startable:
                        _startables.Add(startable);
                        break;
                    case ITickable updatable:
                        _tickables.Add(updatable);
                        break;
                    case IFixedTickable fixedUpdatable:
                        _fixedTickables.Add(fixedUpdatable);
                        break;
                    case ILateTickable lateUpdatable:
                        _lateTickables.Add(lateUpdatable);
                        break;
                }
            }
        }

        private void Start()
        {
            foreach (var startable in _startables)
            {
                startable.Start();
            }
        }
        
        private void Update()
        {
            foreach (var updatable in _tickables)
            {
                updatable.Tick();
            }
        }
        
        private void FixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedTickables)
            {
                fixedUpdatable.FixedTick();
            }
        }
        
        private void LateUpdate()
        {
            foreach (var lateUpdatable in _lateTickables)
            {
                lateUpdatable.LateTick();
            }
        }
    }
}