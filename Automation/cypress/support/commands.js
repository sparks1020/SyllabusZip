// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (username, password) => {})
    Cypress.Commands.add('Login', () => {
    cy.get('a[href*="/Account/Login"]').click()
    cy.get('.form-control[type= email]').type(Cypress.env('username'))
  
    cy.get('.form-control[type= password]').type(Cypress.env('password'))
    cy.get('.btn.btn-primary[type=submit]').click()
  })

  Cypress.Commands.add('Logout', () => {
    cy.get('.nav-link.btn.btn-link.text-info').click()
  })
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
