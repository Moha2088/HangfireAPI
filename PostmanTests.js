pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response time should be less than 10 ms",
    function () {
        pm.expect(pm.response.responseTime).to.be.below(10)
    });

pm.test("Body should contain fields: id, name and password", function () {
    pm.expect(pm.response.text()).to.include("id" && "nasme" && "password")
});

pm.test("Response body should contain string: 'TestUser'", function () {
    pm.expect(pm.response.text()).to.include("TestUser")
});