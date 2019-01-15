Check UM Filter
==================================

Please add this filter to your Class/Method for checking UM 

.. code:: csharp

        [CheckUMFilter]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
        ...
        }
